using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Tcp
{
    public class TcpClientConcurrencySync : IDisposable
    {
        TcpClient client;
        Action<IPacket> callback;
        bool disposedValue;  // 중복호출 제거용

        public TcpClientConcurrencySync(string serverIP, int serverPort, Action<IPacket> callback)
        {
            this.callback = callback;
            this.client = new TcpClient(hostname: serverIP, port: serverPort);
            this.disposedValue = false;
        }

        // TODO: 아래의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다. 
        ~TcpClientConcurrencySync()
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

        public void Start(string stageID, string userID)
        {
            // 1. 접속할 사용자 정보를 보낸다.
            // protocol에 stage id를 넣고, value에 user id를 넣는다.
            PacketValue packet = new PacketValue(protocol: stageID, value: userID);
            Send(packet: packet);

            // 그 후에 서버에서 오는 메세지를 듣기 위해 별도의 쓰레드를 돌린다.
            Thread thread = new Thread(Receive);
            thread.IsBackground = true;
            thread.Start();
        }

        void Receive()
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
                    nbytes = stream.Read(buffer: receiveHeader, offset: 0, size: receiveHeader.Length);

                    // 2. 결과의 데이터를 받는다.
                    receiveDataLength = BitConverter.ToInt32(value: receiveHeader, startIndex: 0);
                    receiveData = TcpUtil.ReceiveData(networkStream: stream, dataLength: receiveDataLength);

                    stream.Flush();

                    // 3. 결과는 압축되어 있으므로 푼다.
                    decompressData = NetUtil.Decompress(data: receiveData);
                    received = NetUtil.DeserializeObject(data: decompressData) as IPacket;

                    this.callback(received);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                    Dispose();
                }
            }
        }

        public void Send(IPacket packet)
        {
            try
            {
                NetworkStream stream = this.client.GetStream();

                // 1. 보낼 데이터를 압축한다.
                byte[] sendData = NetUtil.SerializeObject(data: packet);
                byte[] compressData = NetUtil.Compress(data: sendData);

                // 2. 요청의 헤더를 보낸다.
                int sendDataLength = compressData.Length;
                byte[] sendHeader = BitConverter.GetBytes(value: sendDataLength);
                stream.Write(buffer: sendHeader, offset: 0, size: sendHeader.Length);

                // 3. 요청을 보낸다.
                TcpUtil.SendData(networkStream: stream, data: compressData, dataLength: sendDataLength);

                stream.Flush();
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public class TcpClientConcurrencySyncCollection : List<TcpClientConcurrencySync>
    {
        public TcpClientConcurrencySyncCollection()
        {

        }
    }
}
