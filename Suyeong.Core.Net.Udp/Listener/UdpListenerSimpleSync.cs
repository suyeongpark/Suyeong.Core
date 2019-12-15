using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Udp
{
    public class UdpListenerSimpleSync : IDisposable
    {
        UdpClient listener;
        bool listenOn;

        public UdpListenerSimpleSync(int portNum)
        {
            this.listener = new UdpClient(portNum);
        }

        public void Dispose()
        {
            this.listener.Close();
        }

        public void Start(Func<IPacket, IPacket> callback)
        {
            listenOn = true;

            IPacket receivePacket, sendPacket;
            byte[] sendData, receiveData, compressData, decompressData;
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (this.listenOn)
            {
                try
                {
                    // 1. 요청을 받는다.
                    receiveData = listener.Receive(remoteEP: ref clientEndPoint);

                    // 2. 요청은 압축되어 있으므로 푼다.
                    decompressData = NetUtil.Decompress(data: receiveData);
                    receivePacket = NetUtil.DeserializeObject(data: decompressData) as IPacket;

                    // 3. 요청을 처리한다.
                    sendPacket = callback(receivePacket);

                    // 4. 처리 결과를 압축한다.
                    sendData = NetUtil.SerializeObject(data: sendPacket);
                    compressData = NetUtil.Compress(data: sendData);

                    // 5. 요청을 보내온 곳으로 결과를 보낸다.
                    listener.Send(dgram: compressData, bytes: compressData.Length, endPoint: clientEndPoint);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }

    public class UdpListenerSimpleSyncs : List<UdpListenerSimpleSync>
    {
        public UdpListenerSimpleSyncs()
        {

        }
    }
}
