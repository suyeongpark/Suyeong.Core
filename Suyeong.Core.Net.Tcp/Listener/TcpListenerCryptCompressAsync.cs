using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Tcp
{
    public class TcpListenerCryptCompressAsync
    {
        TcpListener listener;
        byte[] key, iv;

        public TcpListenerCryptCompressAsync(int portNum, byte[] key, byte[] iv)
        {
            this.key = key;
            this.iv = iv;
            this.listener = new TcpListener(new IPEndPoint(address: IPAddress.Any, port: portNum));
        }

        async public Task ListenerStartAsync(Func<IPacket, Task<IPacket>> callback)
        {
            this.listener.Start();

            IPacket receivePacket, sendPacket;
            PacketType type;
            int receiveDataLength, sendDataLength, nbytes;
            byte[] receiveHeader, sendHeader, receiveData, sendData, decryptData, encryptData;

            while (true)
            {
                try
                {
                    using (TcpClient client = await this.listener.AcceptTcpClientAsync().ConfigureAwait(false))
                    using (NetworkStream stream = client.GetStream())
                    {
                        // 1. 요청 헤더를 받는다.
                        receiveHeader = new byte[Consts.SIZE_HEADER];
                        nbytes = await stream.ReadAsync(buffer: receiveHeader, offset: 0, count: receiveHeader.Length);

                        if (nbytes > 0)
                        {
                            // 2. 요청 데이터를 받는다.
                            type = (PacketType)BitConverter.ToInt32(value: receiveHeader, startIndex: 0);
                            receiveDataLength = BitConverter.ToInt32(value: receiveHeader, startIndex: Consts.SIZE_INDEX);  // BitConverter.ToInt32 자체가 4바이트를 읽겠다는 의미라서 Start Index만 있으면 된다.
                            receiveData = await TcpUtil.ReceiveDataAsync(networkStream: stream, dataLength: receiveDataLength);

                            // 3. 받은 요청은 암호화되어 있으므로 푼다.
                            decryptData = await NetUtil.DecryptWithDecompressAsync(data: receiveData, key: this.key, iv: this.iv);
                            receivePacket = NetUtil.DeserializeObject(data: decryptData) as IPacket;

                            // 4. 요청을 처리한다.
                            sendPacket = await callback(receivePacket);

                            // 5. 처리 결과를 압호화한다.
                            sendData = NetUtil.SerializeObject(data: sendPacket);
                            encryptData = await NetUtil.EncryptWithCompressAsync(data: sendData, key: this.key, iv: this.iv);

                            // 6. 처리한 결과의 헤더를 보낸다.
                            sendDataLength = encryptData.Length;
                            sendHeader = BitConverter.GetBytes(value: sendDataLength);
                            await stream.WriteAsync(buffer: sendHeader, offset: 0, count: sendHeader.Length);

                            // 7. 처리한 결과의 데이터를 보낸다.
                            await TcpUtil.SendDataAsync(networkStream: stream, data: encryptData, dataLength: sendDataLength);

                            // 8. flush
                            await stream.FlushAsync();
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void ListenerStop()
        {
            this.listener.Stop();
        }
    }
}
