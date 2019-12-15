using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Udp
{
    public class UdpClientSimpleSync
    {
        IPEndPoint serverEndPoint;

        public UdpClientSimpleSync(string serverIP, int serverPort)
        {
            this.serverEndPoint = new IPEndPoint(address: IPAddress.Parse(serverIP), port: serverPort);
        }

        public IPacket Send(IPacket sendPacket)
        {
            IPacket receivePacket = default;

            try
            {
                // UDP는 65507를 넘는 데이터는 유실 될 수 있고 순서가 꼬일 수 있기 때문에 그보다 큰 데이터는 보내지 않는 것이 좋다.
                // 그것을 보정하려면 그냥 tcp를 쓰는게 낫다.
                using (UdpClient client = new UdpClient())
                {
                    // 1. 보낼 데이터를 압축한다.
                    byte[] sendData = NetUtil.SerializeObject(data: sendPacket);
                    byte[] compressData = NetUtil.Compress(data: sendData);

                    // 2. 보낸다.
                    client.Send(dgram: compressData, bytes: compressData.Length, endPoint: this.serverEndPoint);

                    // 3. 결과의 데이터를 받는다.
                    IPEndPoint remoteEndPoint = null;
                    byte[] receiveData = client.Receive(remoteEP: ref remoteEndPoint);

                    // 4. 결과는 압축되어 있으므로 푼다.
                    byte[] decompressData = NetUtil.Decompress(data: receiveData);
                    receivePacket = NetUtil.DeserializeObject(data: decompressData) as IPacket;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return receivePacket;
        }
    }

    public class UdpClientSimpleSyncs : List<UdpClientSimpleSync>
    {
        public UdpClientSimpleSyncs()
        {

        }
    }
}
