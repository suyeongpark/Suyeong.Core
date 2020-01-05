using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Tcp
{
    public class TcpListenerSimpleCryptAsync : IDisposable
    {
        TcpListener listener;
        byte[] key, iv;
        bool disposedValue;  // 중복호출 제거용

        public TcpListenerSimpleCryptAsync(int portNum, byte[] key, byte[] iv)
        {
            this.key = key;
            this.iv = iv;
            this.listener = new TcpListener(new IPEndPoint(address: IPAddress.Any, port: portNum));
            this.disposedValue = false;
        }

        // TODO: 아래의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다. 
        ~TcpListenerSimpleCryptAsync()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                // TODO: 관리되는 상태(관리되는 개체)를 삭제
                if (disposing)
                {
                    this.listener.Stop();
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제

                // TODO: 큰 필드를 null로 설정.

                this.disposedValue = true;
            }
        }

        async public Task StartAsync(Func<IPacket, Task<IPacket>> callback)
        {
            this.listener.Start();

            IPacket receivePacket, sendPacket;
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
                        nbytes = await stream.ReadAsync(buffer: receiveHeader, offset: 0, count: receiveHeader.Length).ConfigureAwait(false);

                        // 2. 요청 데이터를 받는다.
                        receiveDataLength = BitConverter.ToInt32(value: receiveHeader, startIndex: 0);
                        receiveData = await TcpUtil.ReceiveDataAsync(networkStream: stream, dataLength: receiveDataLength).ConfigureAwait(false);

                        await stream.FlushAsync().ConfigureAwait(false);

                        // 3. 받은 요청은 암호화되어 있으므로 푼다.
                        decryptData = await NetUtil.DecryptAsync(data: receiveData, key: this.key, iv: this.iv).ConfigureAwait(false);
                        receivePacket = NetUtil.DeserializeObject(data: decryptData) as IPacket;

                        // 4. 요청을 처리한다.
                        sendPacket = await callback(receivePacket).ConfigureAwait(false);

                        // 5. 처리 결과를 암호화한다.
                        sendData = NetUtil.SerializeObject(data: sendPacket);
                        encryptData = await NetUtil.EncryptAsync(data: sendData, key: this.key, iv: this.iv).ConfigureAwait(false);

                        // 6. 처리한 결과의 헤더를 보낸다.
                        sendDataLength = encryptData.Length;
                        sendHeader = BitConverter.GetBytes(value: sendDataLength);
                        await stream.WriteAsync(buffer: sendHeader, offset: 0, count: sendHeader.Length).ConfigureAwait(false);

                        // 7. 처리한 결과의 데이터를 보낸다.
                        await TcpUtil.SendDataAsync(networkStream: stream, data: encryptData, dataLength: sendDataLength).ConfigureAwait(false);

                        await stream.FlushAsync().ConfigureAwait(false);
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }

    public class TcpListenerSimpleCryptAsyncCollection : List<TcpListenerSimpleCryptAsync>
    {
        public TcpListenerSimpleCryptAsyncCollection()
        {

        }
    }
}
