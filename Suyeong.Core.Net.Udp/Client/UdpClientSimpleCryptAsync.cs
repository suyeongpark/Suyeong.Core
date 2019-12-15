using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Udp
{
    public class UdpClientSimpleCryptAsync
    {
        IPEndPoint serverEndPoint;
        byte[] key, iv;

        public UdpClientSimpleCryptAsync(string serverIP, int serverPort, byte[] key, byte[] iv)
        {
            this.serverEndPoint = new IPEndPoint(address: IPAddress.Parse(serverIP), port: serverPort);
            this.key = key;
            this.iv = iv;
        }

        async public Task<IPacket> Send(IPacket sendPacket)
        {
            IPacket receivePacket = default;

            try
            {
                // UDP는 65507를 넘는 데이터는 유실 될 수 있고 순서가 꼬일 수 있기 때문에 그보다 큰 데이터는 보내지 않는 것이 좋다.
                // 그것을 보정하려면 그냥 tcp를 쓰는게 낫다.
                using (UdpClient client = new UdpClient())
                {
                    // 1. 보낼 데이터를 암호화한다.
                    byte[] sendData = NetUtil.SerializeObject(data: sendPacket);
                    byte[] encryptData = await NetUtil.EncryptAsync(data: sendData, key: this.key, iv: this.iv);

                    // 2. 보낸다.
                    await client.SendAsync(datagram: encryptData, bytes: encryptData.Length, endPoint: this.serverEndPoint);

                    // 3. 결과의 데이터를 받는다.
                    UdpReceiveResult result = await client.ReceiveAsync();

                    // 4. 결과는 암호화되어 있으므로 푼다.
                    byte[] decryptData = await NetUtil.DecryptAsync(data: result.Buffer, key: this.key, iv: this.iv);
                    receivePacket = NetUtil.DeserializeObject(data: decryptData) as IPacket;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return receivePacket;
        }
    }

    public class UdpClientSimpleCryptAsyncs : List<UdpClientSimpleCryptAsync>
    {
        public UdpClientSimpleCryptAsyncs()
        {

        }
    }
}
