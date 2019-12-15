using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Udp
{
    public class UdpListenerSimpleCryptAsync : IDisposable
    {
        UdpClient listener;
        byte[] key, iv;
        bool listenOn;

        public UdpListenerSimpleCryptAsync(int portNum, byte[] key, byte[] iv)
        {
            this.listener = new UdpClient(portNum);
            this.key = key;
            this.iv = iv;
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
            byte[] sendData, decryptData, encryptData;

            while (this.listenOn)
            {
                try
                {
                    // 1. 요청을 받는다.
                    result = await listener.ReceiveAsync();

                    // 2. 요청은 암호화되어 있으므로 푼다.
                    decryptData = await NetUtil.DecryptAsync(data: result.Buffer, key: this.key, iv: this.iv);
                    receivePacket = NetUtil.DeserializeObject(data: decryptData) as IPacket;

                    // 3. 요청을 처리한다.
                    sendPacket = await callback(receivePacket);

                    // 4. 처리 결과를 암호화한다.
                    sendData = NetUtil.SerializeObject(data: sendPacket);
                    encryptData = await NetUtil.EncryptAsync(data: sendData, key: this.key, iv: this.iv);

                    // 5. 요청을 보내온 곳으로 결과를 보낸다.
                    await listener.SendAsync(datagram: encryptData, bytes: encryptData.Length, endPoint: result.RemoteEndPoint);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }

    public class UdpListenerSimpleCryptAsyncs : List<UdpListenerSimpleCryptAsync>
    {
        public UdpListenerSimpleCryptAsyncs()
        {

        }
    }
}
