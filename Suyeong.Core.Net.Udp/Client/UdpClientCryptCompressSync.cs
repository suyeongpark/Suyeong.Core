using System;
using System.Net;
using System.Net.Sockets;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Udp
{
    public class UdpClientCryptCompressSync
    {
        IPEndPoint serverEndPoint, clientEndPoint;
        byte[] key, iv;

        public UdpClientCryptCompressSync(string serverIP, int serverPort, byte[] key, byte[] iv, int clientPort = 0)
        {
            this.serverEndPoint = new IPEndPoint(address: IPAddress.Parse(serverIP), port: serverPort);
            this.clientEndPoint = new IPEndPoint(address: IPAddress.Any, port: clientPort);
            this.key = key;
            this.iv = iv;
        }

        public void Send(IPacket sendPacket, Action<IPacket> callback)
        {
            IPacket receivePacket;
            byte[] sendData, receiveData, compressData, decompressData;

            try
            {
                // UDP는 65507를 넘는 데이터는 유실 될 수 있고 순서가 꼬일 수 있기 때문에 그보다 큰 데이터는 보내지 않는 것이 좋다.
                // 그것을 보정하려면 그냥 tcp를 쓰는게 낫다.
                using (UdpClient client = new UdpClient())
                {
                    // 1. 보낼 데이터를 암호화한다.
                    sendData = NetUtil.SerializeObject(data: sendPacket);
                    compressData = NetUtil.EncryptWithCompress(data: sendData, key: this.key, iv: this.iv);

                    // 2. 보낸다.
                    client.Send(dgram: compressData, bytes: compressData.Length, endPoint: this.serverEndPoint);

                    // 3. 결과의 데이터를 받는다.
                    receiveData = client.Receive(remoteEP: ref this.clientEndPoint);

                    // 4. 결과는 암호화되어 있으므로 푼다.
                    decompressData = NetUtil.DecryptWithDecompress(data: receiveData, key: this.key, iv: this.iv);
                    receivePacket = NetUtil.DeserializeObject(data: decompressData) as IPacket;

                    // 5. 결과를 처리한다.
                    callback(receivePacket);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
