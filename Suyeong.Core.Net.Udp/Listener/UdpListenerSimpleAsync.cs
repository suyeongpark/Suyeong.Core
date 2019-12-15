using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Udp
{
    public class UdpListenerSimpleAsync : IDisposable
    {
        UdpClient listener;
        bool listenOn;

        public UdpListenerSimpleAsync(int portNum)
        {
            this.listener = new UdpClient(portNum);
        }

        public void Dispose()
        {
            this.listener.Close();
        }

        async public Task Start(Func<IPacket, Task<IPacket>> callback)
        {
            listenOn = true;

            IPacket receivePacket, sendPacket;
            UdpReceiveResult result;
            byte[] sendData, compressData, decompressData;

            while (this.listenOn)
            {
                try
                {
                    // 1. 요청을 받는다.
                    result = await listener.ReceiveAsync();

                    // 2. 요청은 압축되어 있으므로 푼다.
                    decompressData = await NetUtil.DecompressAsync(data: result.Buffer);
                    receivePacket = NetUtil.DeserializeObject(data: decompressData) as IPacket;

                    // 3. 요청을 처리한다.
                    sendPacket = await callback(receivePacket);

                    // 4. 처리 결과를 압축한다.
                    sendData = NetUtil.SerializeObject(data: sendPacket);
                    compressData = await NetUtil.CompressAsync(data: sendData);

                    // 5. 요청을 보내온 곳으로 결과를 보낸다.
                    await listener.SendAsync(datagram: compressData, bytes: compressData.Length, endPoint: result.RemoteEndPoint);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }

    public class UdpListenerSimpleAsyncs : List<UdpListenerSimpleAsync>
    {
        public UdpListenerSimpleAsyncs()
        {

        }
    }
}
