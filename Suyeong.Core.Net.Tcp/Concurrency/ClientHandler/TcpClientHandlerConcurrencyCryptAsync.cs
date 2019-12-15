using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Tcp
{
    public class TcpClientHandlerConcurrencyCryptAsync : IDisposable
    {
        public event Func<string, IPacket, Task> Receive;
        public event Func<string, string, Task> Disconnect;

        TcpClient client;
        string stageID, userID;
        byte[] key, iv;

        public TcpClientHandlerConcurrencyCryptAsync(TcpClient client, string stageID, string userID, byte[] key, byte[] iv)
        {
            this.client = client;
            this.stageID = stageID;
            this.userID = userID;
            this.key = key;
            this.iv = iv;
        }

        public bool Connected { get { return this.client.Connected; } }

        public void Dispose()
        {
            if (this.client.Connected)
            {
                this.client.Close();
            }
        }

        public void SetStageID (string stageID)
        {
            this.stageID = stageID;
        }

        async public Task StartAsync()
        {
            NetworkStream stream;
            IPacket received;
            byte[] receiveHeader, receiveData, decryptData;
            int nbytes, receiveDataLength;

            // 클라이언트에서 오는 메세지를 듣기 위해 별도의 쓰레드를 돌린다.
            await Task.Factory.StartNew(async () =>
            {
                while (this.client.Connected)
                {
                    try
                    {
                        stream = this.client.GetStream();

                        // 1. 결과의 헤더를 받는다.
                        receiveHeader = new byte[Consts.SIZE_HEADER];
                        nbytes = await stream.ReadAsync(buffer: receiveHeader, offset: 0, count: receiveHeader.Length);

                        // 2. 결과의 데이터를 받는다.
                        receiveDataLength = BitConverter.ToInt32(value: receiveHeader, startIndex: 0);
                        receiveData = await TcpUtil.ReceiveDataAsync(networkStream: stream, dataLength: receiveDataLength);

                        await stream.FlushAsync();

                        // 3. 결과는 암호화되어 있으므로 푼다.
                        decryptData = await NetUtil.DecryptAsync(data: receiveData, key: this.key, iv: this.iv);
                        received = NetUtil.DeserializeObject(data: decryptData) as IPacket;

                        await Receive(this.stageID, received);
                    }
                    catch (SocketException)
                    {
                        await Disconnect(this.stageID, this.userID);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        await Disconnect(this.stageID, this.userID);
                    }
                }
            });
        }

        async public Task SendAsync(IPacket packet)
        {
            try
            {
                NetworkStream stream = this.client.GetStream();

                // 1. 보낼 데이터를 암호화한다.
                byte[] sendData = NetUtil.SerializeObject(data: packet);
                byte[] encryptData = await NetUtil.EncryptAsync(data: sendData, key: this.key, iv: this.iv);

                // 2. 요청의 헤더를 보낸다.
                int sendDataLength = encryptData.Length;
                byte[] sendHeader = BitConverter.GetBytes(value: sendDataLength);
                await stream.WriteAsync(buffer: sendHeader, offset: 0, count: sendHeader.Length);

                // 3. 요청을 보낸다.
                await TcpUtil.SendDataAsync(networkStream: stream, data: encryptData, dataLength: sendDataLength);

                await stream.FlushAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public class TcpClientHandlerConcurrencyCryptAsyncDic : Dictionary<string, TcpClientHandlerConcurrencyCryptAsync>
    {
        public TcpClientHandlerConcurrencyCryptAsyncDic()
        {

        }
    }

    public class TcpClientHandlerConcurrencyCryptAsyncDicGroup : Dictionary<string, TcpClientHandlerConcurrencyCryptAsyncDic>
    {
        public TcpClientHandlerConcurrencyCryptAsyncDicGroup()
        {

        }
    }
}
