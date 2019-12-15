using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Tcp
{
    public class TcpClientSimpleCryptAsync
    {
        string serverIP;
        int serverPort;
        byte[] key, iv;

        public TcpClientSimpleCryptAsync(string serverIP, int serverPort, byte[] key, byte[] iv)
        {
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            this.key = key;
            this.iv = iv;
        }

        async public Task<IPacket> Send(IPacket sendPacket)
        {
            IPacket receivePacket = default;

            try
            {
                using (TcpClient client = new TcpClient(hostname: this.serverIP, port: this.serverPort))
                using (NetworkStream stream = client.GetStream())
                {
                    // 1. 보낼 데이터를 암호화한다.
                    byte[] sendData = NetUtil.SerializeObject(data: sendPacket);
                    byte[] encryptData = await NetUtil.EncryptAsync(data: sendData, key: this.key, iv: this.iv);

                    // 2. 요청의 헤더를 보낸다.
                    int sendDataLength = encryptData.Length;
                    byte[] sendHeader = BitConverter.GetBytes(value: sendDataLength);
                    await stream.WriteAsync(buffer: sendHeader, offset: 0, count: sendHeader.Length);

                    // 3. 요청을 보낸다.
                    await TcpUtil.SendDataAsync(networkStream: stream, data: encryptData, dataLength: sendDataLength);

                    await stream.FlushAsync();

                    // 4. 결과의 헤더를 받는다.
                    byte[] receiveHeader = new byte[Consts.SIZE_HEADER];
                    int nbytes = await stream.ReadAsync(buffer: receiveHeader, offset: 0, count: receiveHeader.Length);

                    // 5. 결과의 데이터를 받는다.
                    int receiveDataLength = BitConverter.ToInt32(value: receiveHeader, startIndex: 0);
                    byte[] receiveData = await TcpUtil.ReceiveDataAsync(networkStream: stream, dataLength: receiveDataLength);

                    await stream.FlushAsync();

                    // 6. 결과는 암호화되어 있으므로 푼다.
                    byte[] decryptData = await NetUtil.DecryptAsync(data: receiveData, key: this.key, iv: this.iv);
                    receivePacket = NetUtil.DeserializeObject(data: decryptData) as IPacket;

                    // 7. 결과를 처리한다.
                    return receivePacket;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return receivePacket;
        }
    }

    public class TcpClientSimpleCryptAsyncs : List<TcpClientSimpleCryptAsync>
    {
        public TcpClientSimpleCryptAsyncs()
        {

        }
    }
}
