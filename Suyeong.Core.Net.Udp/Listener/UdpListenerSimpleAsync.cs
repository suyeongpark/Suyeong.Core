using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Udp
{
    public class UdpListenerSimpleAsync : IDisposable
    {
        UdpClient listener;
        bool listenOn;
        bool disposedValue;  // 중복호출 제거용

        public UdpListenerSimpleAsync(int portNum)
        {
            this.listener = new UdpClient(portNum);
            this.disposedValue = false;
        }

        // TODO: 아래의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다. 
        ~UdpListenerSimpleAsync()
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
                    this.listener.Close();
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제

                // TODO: 큰 필드를 null로 설정.

                this.disposedValue = true;
            }
        }

        async public Task Start(Func<IPacket, Task<IPacket>> callback)
        {
            listenOn = true;

            IPacket receivePacket, sendPacket;
            UdpReceiveResult result;
            byte[] sendData, compressData, decompressData;

            while (this.listenOn)
            {
                try
                {
                    // 1. 요청을 받는다.
                    result = await listener.ReceiveAsync().ConfigureAwait(false);

                    // 2. 요청은 압축되어 있으므로 푼다.
                    decompressData = await NetUtil.DecompressAsync(data: result.Buffer).ConfigureAwait(false);
                    receivePacket = NetUtil.DeserializeObject(data: decompressData) as IPacket;

                    // 3. 요청을 처리한다.
                    sendPacket = await callback(receivePacket).ConfigureAwait(false);

                    // 4. 처리 결과를 압축한다.
                    sendData = NetUtil.SerializeObject(data: sendPacket);
                    compressData = await NetUtil.CompressAsync(data: sendData).ConfigureAwait(false);

                    // 5. 요청을 보내온 곳으로 결과를 보낸다.
                    await listener.SendAsync(datagram: compressData, bytes: compressData.Length, endPoint: result.RemoteEndPoint).ConfigureAwait(false);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }

    public class UdpListenerSimpleAsyncCollection : List<UdpListenerSimpleAsync>
    {
        public UdpListenerSimpleAsyncCollection()
        {

        }
    }
}
