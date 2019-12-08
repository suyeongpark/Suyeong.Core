using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Udp
{
    public class UdpClientAsync
    {
        IPEndPoint serverEndPoint;

        public UdpClientAsync(string serverIP, int serverPort)
        {
            this.serverEndPoint = new IPEndPoint(address: IPAddress.Parse(serverIP), port: serverPort);
        }

        async public Task Send(IPacket sendPacket, Action<IPacket> callback)
        {
            IPacket receivePacket;
            UdpReceiveResult result;
            byte[] sendData, compressData, decompressData;

            try
            {
                // UDP는 65507를 넘는 데이터는 유실 될 수 있고 순서가 꼬일 수 있기 때문에 그보다 큰 데이터는 보내지 않는 것이 좋다.
                // 그것을 보정하려면 그냥 tcp를 쓰는게 낫다.
                using (UdpClient client = new UdpClient())
                {
                    // 1. 보낼 데이터를 압축한다.
                    sendData = NetUtil.SerializeObject(data: sendPacket);
                    compressData = await NetUtil.CompressAsync(data: sendData);

                    // 2. 보낸다.
                    await client.SendAsync(datagram: compressData, bytes: compressData.Length, endPoint: this.serverEndPoint);

                    // 3. 결과의 데이터를 받는다.
                    result = await client.ReceiveAsync();

                    // 4. 결과는 압축되어 있으므로 푼다.
                    decompressData = await NetUtil.DecompressAsync(data: result.Buffer);
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
