using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Udp
{
    public class UdpClientCryptCompressAsync
    {
        IPEndPoint serverEndPoint;
        byte[] key, iv;

        public UdpClientCryptCompressAsync(string serverIP, int serverPort, byte[] key, byte[] iv)
        {
            this.serverEndPoint = new IPEndPoint(address: IPAddress.Parse(serverIP), port: serverPort);
            this.key = key;
            this.iv = iv;
        }

        async public Task Send(IPacket sendPacket, Action<IPacket> callback)
        {
            IPacket receivePacket;
            UdpReceiveResult result;
            byte[] sendData, encryptData, decryptData;

            try
            {
                // UDP는 65507를 넘는 데이터는 유실 될 수 있고 순서가 꼬일 수 있기 때문에 그보다 큰 데이터는 보내지 않는 것이 좋다.
                // 그것을 보정하려면 그냥 tcp를 쓰는게 낫다.
                using (UdpClient client = new UdpClient())
                {
                    // 1. 보낼 데이터를 암호화한다.
                    sendData = NetUtil.SerializeObject(data: sendPacket);
                    encryptData = await NetUtil.EncryptWithCompressAsync(data: sendData, key: this.key, iv: this.iv);

                    // 2. 보낸다.
                    await client.SendAsync(datagram: encryptData, bytes: encryptData.Length, endPoint: this.serverEndPoint);

                    // 3. 결과의 데이터를 받는다.
                    result = await client.ReceiveAsync();

                    // 4. 결과는 압축되어 있으므로 푼다.
                    decryptData = await NetUtil.DecryptWithDecompressAsync(data: result.Buffer, key: this.key, iv: this.iv);
                    receivePacket = NetUtil.DeserializeObject(data: decryptData) as IPacket;

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
