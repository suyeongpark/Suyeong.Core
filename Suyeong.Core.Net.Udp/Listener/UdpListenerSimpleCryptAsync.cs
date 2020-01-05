using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Udp
{
    public class UdpListenerSimpleCryptAsync : IDisposable
    {
        UdpClient listener;
        byte[] key, iv;
        bool listenOn;
        bool disposedValue;  // 중복호출 제거용

        public UdpListenerSimpleCryptAsync(int portNum, byte[] key, byte[] iv)
        {
            this.listener = new UdpClient(portNum);
            this.key = key;
            this.iv = iv;
            this.disposedValue = false;
        }

        // TODO: 아래의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다. 
        ~UdpListenerSimpleCryptAsync()
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
            byte[] sendData, decryptData, encryptData;

            while (this.listenOn)
            {
                try
                {
                    // 1. 요청을 받는다.
                    result = await listener.ReceiveAsync().ConfigureAwait(false);

                    // 2. 요청은 암호화되어 있으므로 푼다.
                    decryptData = await NetUtil.DecryptAsync(data: result.Buffer, key: this.key, iv: this.iv).ConfigureAwait(false);
                    receivePacket = NetUtil.DeserializeObject(data: decryptData) as IPacket;

                    // 3. 요청을 처리한다.
                    sendPacket = await callback(receivePacket).ConfigureAwait(false);

                    // 4. 처리 결과를 암호화한다.
                    sendData = NetUtil.SerializeObject(data: sendPacket);
                    encryptData = await NetUtil.EncryptAsync(data: sendData, key: this.key, iv: this.iv).ConfigureAwait(false);

                    // 5. 요청을 보내온 곳으로 결과를 보낸다.
                    await listener.SendAsync(datagram: encryptData, bytes: encryptData.Length, endPoint: result.RemoteEndPoint).ConfigureAwait(false);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }

    public class UdpListenerSimpleCryptAsyncCollection : List<UdpListenerSimpleCryptAsync>
    {
        public UdpListenerSimpleCryptAsyncCollection()
        {

        }
    }
}
