using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Tcp
{
    public class TcpClientSimpleAsync
    {
        string serverIP;
        int serverPort;

        public TcpClientSimpleAsync(string serverIP, int serverPort)
        {
            this.serverIP = serverIP;
            this.serverPort = serverPort;
        }

        async public Task<IPacket> Send(IPacket sendPacket)
        {
            IPacket receivePacket = default;

            try
            {
                using (TcpClient client = new TcpClient(hostname: this.serverIP, port: this.serverPort))
                using (NetworkStream stream = client.GetStream())
                {
                    // 1. 보낼 데이터를 압축한다.
                    byte[] sendData = NetUtil.SerializeObject(data: sendPacket);
                    byte[] compressData = await NetUtil.CompressAsync(data: sendData);

                    // 2. 요청의 헤더를 보낸다.
                    int sendDataLength = compressData.Length;
                    byte[] sendHeader = BitConverter.GetBytes(value: sendDataLength);
                    await stream.WriteAsync(buffer: sendHeader, offset: 0, count: sendHeader.Length);

                    // 3. 요청을 보낸다.
                    await TcpUtil.SendDataAsync(networkStream: stream, data: compressData, dataLength: sendDataLength);

                    await stream.FlushAsync();

                    // 4. 결과의 헤더를 받는다.
                    byte[] receiveHeader = new byte[Consts.SIZE_HEADER];
                    int nbytes = await stream.ReadAsync(buffer: receiveHeader, offset: 0, count: receiveHeader.Length);

                    // 5. 결과의 데이터를 받는다.
                    int receiveDataLength = BitConverter.ToInt32(value: receiveHeader, startIndex: 0);
                    byte[] receiveData = await TcpUtil.ReceiveDataAsync(networkStream: stream, dataLength: receiveDataLength);

                    await stream.FlushAsync();

                    // 6. 결과는 압축되어 있으므로 푼다.
                    byte[] decompressData = await NetUtil.DecompressAsync(data: receiveData);
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

    public class TcpClientSimpleAsyncs : List<TcpClientSimpleAsync>
    {
        public TcpClientSimpleAsyncs()
        {

        }
    }
}
