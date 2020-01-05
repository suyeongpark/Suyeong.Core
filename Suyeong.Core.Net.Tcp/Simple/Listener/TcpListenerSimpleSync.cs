using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Tcp
{
    public class TcpListenerSimpleSync : IDisposable
    {
        TcpListener listener;
        bool disposedValue;  // 중복호출 제거용

        public TcpListenerSimpleSync(int portNum)
        {
            this.listener = new TcpListener(new IPEndPoint(address: IPAddress.Any, port: portNum));
            this.disposedValue = false;
        }

        // TODO: 아래의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다. 
        ~TcpListenerSimpleSync()
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

        public void Start(Func<IPacket, IPacket> callback)
        {
            this.listener.Start();

            IPacket receivePacket, sendPacket;
            int receiveDataLength, sendDataLength, nbytes;
            byte[] receiveHeader, sendHeader, receiveData, sendData, decompressData, compressData;

            while (true)
            {
                try
                {
                    using (TcpClient client = this.listener.AcceptTcpClient())
                    using (NetworkStream stream = client.GetStream())
                    {
                        // 1. 요청 헤더를 받는다.
                        receiveHeader = new byte[Consts.SIZE_HEADER];
                        nbytes = stream.Read(buffer: receiveHeader, offset: 0, size: receiveHeader.Length);

                        // 2. 요청 데이터를 받는다.
                        receiveDataLength = BitConverter.ToInt32(value: receiveHeader, startIndex: 0);
                        receiveData = TcpUtil.ReceiveData(networkStream: stream, dataLength: receiveDataLength);

                        stream.Flush();

                        // 3. 받은 요청은 압축되어 있으므로 푼다.
                        decompressData = NetUtil.Decompress(data: receiveData);
                        receivePacket = NetUtil.DeserializeObject(data: decompressData) as IPacket;

                        // 4. 요청을 처리한다.
                        sendPacket = callback(receivePacket);

                        // 5. 처리 결과를 압축한다.
                        sendData = NetUtil.SerializeObject(data: sendPacket);
                        compressData = NetUtil.Compress(data: sendData);

                        // 6. 처리한 결과의 헤더를 보낸다.
                        sendDataLength = compressData.Length;
                        sendHeader = BitConverter.GetBytes(value: sendDataLength);
                        stream.Write(buffer: sendHeader, offset: 0, size: sendHeader.Length);

                        // 7. 처리한 결과의 데이터를 보낸다.
                        TcpUtil.SendData(networkStream: stream, data: compressData, dataLength: sendDataLength);

                        stream.Flush();
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }

    public class TcpListenerSimpleSyncCollection : List<TcpListenerSimpleSync>
    {
        public TcpListenerSimpleSyncCollection()
        {

        }
    }
}
