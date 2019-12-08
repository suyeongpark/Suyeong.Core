using System;
using System.Net;
using System.Net.Sockets;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Udp
{
    public class UdpListenerCryptCompressSync
    {
        UdpClient listener;
        byte[] key, iv;
        bool listenOn;

        public UdpListenerCryptCompressSync(int portNum, byte[] key, byte[] iv)
        {
            this.listener = new UdpClient(portNum);
            this.key = key;
            this.iv = iv;
        }

        public void ListenerStart(Func<IPacket, IPacket> callback)
        {
            listenOn = true;

            IPacket receivePacket, sendPacket;
            byte[] receiveData, sendData, decryptData, encryptData;
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (this.listenOn)
            {
                try
                {
                    // 1. 요청을 받는다.
                    receiveData = listener.Receive(remoteEP: ref clientEndPoint);

                    // 2. 요청은 압축되어 있으므로 푼다.
                    decryptData = NetUtil.DecryptWithDecompress(data: receiveData, key: this.key, iv: this.iv);
                    receivePacket = NetUtil.DeserializeObject(data: decryptData) as IPacket;

                    // 3. 요청을 처리한다.
                    sendPacket = callback(receivePacket);

                    // 4. 처리 결과를 압축한다.
                    sendData = NetUtil.SerializeObject(data: sendPacket);
                    encryptData = NetUtil.EncryptWithCompress(data: sendData, key: this.key, iv: this.iv);

                    // 5. 요청을 보내온 곳으로 결과를 보낸다.
                    listener.Send(dgram: encryptData, bytes: encryptData.Length, endPoint: clientEndPoint);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void ListenerStop()
        {
            listenOn = false;
        }
    }
}
