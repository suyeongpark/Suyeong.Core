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

        public IPEndPoint ServerEndPoint { get { return new IPEndPoint(address: IPAddress.Parse(this.serverIP), port: this.serverPort); } }

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
                    byte[] compressData = await NetUtil.CompressAsync(data: sendData).ConfigureAwait(false);

                    // 2. 요청의 헤더를 보낸다.
                    int sendDataLength = compressData.Length;
                    byte[] sendHeader = BitConverter.GetBytes(value: sendDataLength);
                    await stream.WriteAsync(buffer: sendHeader, offset: 0, count: sendHeader.Length).ConfigureAwait(false);

                    // 3. 요청을 보낸다.
                    await TcpUtil.SendDataAsync(networkStream: stream, data: compressData, dataLength: sendDataLength).ConfigureAwait(false);

                    await stream.FlushAsync().ConfigureAwait(false);

                    // 4. 결과의 헤더를 받는다.
                    byte[] receiveHeader = new byte[Consts.SIZE_HEADER];
                    int nbytes = await stream.ReadAsync(buffer: receiveHeader, offset: 0, count: receiveHeader.Length).ConfigureAwait(false);

                    // 5. 결과의 데이터를 받는다.
                    int receiveDataLength = BitConverter.ToInt32(value: receiveHeader, startIndex: 0);
                    byte[] receiveData = await TcpUtil.ReceiveDataAsync(networkStream: stream, dataLength: receiveDataLength).ConfigureAwait(false);

                    await stream.FlushAsync().ConfigureAwait(false);

                    // 6. 결과는 압축되어 있으므로 푼다.
                    byte[] decompressData = await NetUtil.DecompressAsync(data: receiveData).ConfigureAwait(false);
                    receivePacket = NetUtil.DeserializeObject(data: decompressData) as IPacket;
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
            }

            return receivePacket;
        }
    }

    public class TcpClientSimpleAsyncCollection : List<TcpClientSimpleAsync>
    {
        public TcpClientSimpleAsyncCollection()
        {

        }
    }
}
