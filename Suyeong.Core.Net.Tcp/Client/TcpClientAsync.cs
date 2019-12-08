using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Tcp
{
    public class TcpClientAsync
    {
        IPEndPoint serverEndPoint;

        public TcpClientAsync(string serverIP, int serverPort)
        {
            this.serverEndPoint = new IPEndPoint(address: IPAddress.Parse(serverIP), port: serverPort);
        }

        async public Task Send(IPacket sendPacket, Action<IPacket> callback)
        {
            IPacket receivePacket;
            PacketType type;
            int sendDataLength, receiveDataLength, nbytes;
            byte[] sendHeader, receiveHeader, sendData, receiveData, compressData, decompressData;

            try
            {
                using (TcpClient client = new TcpClient(localEP: this.serverEndPoint))
                using (NetworkStream stream = client.GetStream())
                {
                    // 1. 보낼 데이터를 압축한다.
                    sendData = NetUtil.SerializeObject(data: sendPacket);
                    compressData = await NetUtil.CompressAsync(data: sendData);

                    // 2. 요청의 헤더를 보낸다.
                    sendDataLength = compressData.Length;
                    sendHeader = BitConverter.GetBytes(value: sendDataLength);
                    await stream.WriteAsync(buffer: sendHeader, offset: 0, count: sendHeader.Length);

                    // 3. 요청을 보낸다.
                    await TcpUtil.SendDataAsync(networkStream: stream, data: compressData, dataLength: sendDataLength);

                    // 4. 결과의 헤더를 받는다.
                    receiveHeader = new byte[Consts.SIZE_HEADER];
                    nbytes = await stream.ReadAsync(buffer: receiveHeader, offset: 0, count: receiveHeader.Length);

                    if (nbytes > 0)
                    {
                        // 5. 결과의 데이터를 받는다.
                        type = (PacketType)BitConverter.ToInt32(value: receiveHeader, startIndex: 0);
                        receiveDataLength = BitConverter.ToInt32(value: receiveHeader, startIndex: Consts.SIZE_INDEX);  // BitConverter.ToInt32 자체가 4바이트를 읽겠다는 의미라서 Start Index만 있으면 된다.
                        receiveData = await TcpUtil.ReceiveDataAsync(networkStream: stream, dataLength: receiveDataLength);

                        // 6. 결과는 압축되어 있으므로 푼다.
                        decompressData = await NetUtil.DecompressAsync(data: receiveData);
                        receivePacket = NetUtil.DeserializeObject(data: decompressData) as IPacket;

                        // 7. 결과를 처리한다.
                        callback(receivePacket);
                    }

                    await stream.FlushAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
