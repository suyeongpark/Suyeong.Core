using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Tcp
{
    public class TcpClientConcurrencyAsync : IDisposable
    {
        TcpClient client;
        Func<IPacket, Task> callback;
        bool disposedValue;  // 중복호출 제거용

        public TcpClientConcurrencyAsync(string serverIP, int serverPort, Func<IPacket, Task> callback)
        {
            this.callback = callback;
            this.client = new TcpClient(hostname: serverIP, port: serverPort);
            this.disposedValue = false;
        }

        // TODO: 아래의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다. 
        ~TcpClientConcurrencyAsync()
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
                    if (this.client.Connected)
                    {
                        this.client.Close();
                    }
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제

                // TODO: 큰 필드를 null로 설정.

                this.disposedValue = true;
            }
        }

        public bool Connected { get { return this.client.Connected; } }
        public EndPoint ServerEndPoint { get { return this.client.Client.RemoteEndPoint; } }

        async public Task StartAsync(string stageID, string userID)
        {
            // 1. 접속할 사용자 정보를 보낸다.
            // protocol에 stage id를 넣고, value에 user id를 넣는다.
            PacketValue packet = new PacketValue(protocol: stageID, value: userID);
            await SendAsync(packet: packet).ConfigureAwait(false);

            // 2. 받는다.
            await ReceiveAsync().ConfigureAwait(false);
        }

        async Task ReceiveAsync()
        {
            NetworkStream stream;
            IPacket received;
            byte[] receiveHeader, receiveData, decompressData;
            int nbytes, receiveDataLength;

            while (this.client.Connected)
            {
                try
                {
                    stream = this.client.GetStream();

                    // 1. 결과의 헤더를 받는다.
                    receiveHeader = new byte[Consts.SIZE_HEADER];
                    nbytes = await stream.ReadAsync(buffer: receiveHeader, offset: 0, count: receiveHeader.Length).ConfigureAwait(false);

                    // 2. 결과의 데이터를 받는다.
                    receiveDataLength = BitConverter.ToInt32(value: receiveHeader, startIndex: 0);
                    receiveData = await TcpUtil.ReceiveDataAsync(networkStream: stream, dataLength: receiveDataLength).ConfigureAwait(false);

                    await stream.FlushAsync().ConfigureAwait(false);

                    // 3. 결과는 압축되어 있으므로 푼다.
                    decompressData = await NetUtil.DecompressAsync(data: receiveData).ConfigureAwait(false);
                    received = NetUtil.DeserializeObject(data: decompressData) as IPacket;

                    await this.callback(received).ConfigureAwait(false);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                    Dispose();
                }
            }
        }

        async public Task SendAsync(IPacket packet)
        {
            try
            {
                NetworkStream stream = this.client.GetStream();

                // 1. 보낼 데이터를 압축한다.
                byte[] sendData = NetUtil.SerializeObject(data: packet);
                byte[] compressData = await NetUtil.CompressAsync(data: sendData).ConfigureAwait(false);

                // 2. 요청의 헤더를 보낸다.
                int sendDataLength = compressData.Length;
                byte[] sendHeader = BitConverter.GetBytes(value: sendDataLength);
                await stream.WriteAsync(buffer: sendHeader, offset: 0, count: sendHeader.Length).ConfigureAwait(false);

                // 3. 요청을 보낸다.
                await TcpUtil.SendDataAsync(networkStream: stream, data: compressData, dataLength: sendDataLength).ConfigureAwait(false);

                await stream.FlushAsync().ConfigureAwait(false);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public class TcpClientConcurrencyAsyncCollection : List<TcpClientConcurrencyAsync>
    {
        public TcpClientConcurrencyAsyncCollection()
        {

        }
    }
}
