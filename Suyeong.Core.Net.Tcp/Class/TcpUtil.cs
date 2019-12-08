using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Suyeong.Core.Net.Tcp
{
    public static class TcpUtil
    {
        public static bool SendData(NetworkStream networkStream, byte[] data, int dataLength)
        {
            bool result = false;

            try
            {
                using (MemoryStream memoryStream = new MemoryStream(buffer: data))
                {
                    byte[] buffer;
                    int nbytes = 0;

                    while (dataLength > 0)
                    {
                        buffer = new byte[Consts.SIZE_MAX < dataLength ? Consts.SIZE_MAX : dataLength];
                        nbytes = memoryStream.Read(buffer: buffer, offset: 0, count: buffer.Length);
                        networkStream.Write(buffer: buffer, offset: 0, size: buffer.Length);
                        dataLength -= nbytes;
                    }
                }

                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        async public static Task<bool> SendDataAsync(NetworkStream networkStream, byte[] data, int dataLength)
        {
            bool result = false;

            try
            {
                using (MemoryStream memoryStream = new MemoryStream(buffer: data))
                {
                    byte[] buffer;
                    int nbytes = 0;

                    while (dataLength > 0)
                    {
                        buffer = new byte[Consts.SIZE_MAX < dataLength ? Consts.SIZE_MAX : dataLength];
                        nbytes = await memoryStream.ReadAsync(buffer: buffer, offset: 0, count: buffer.Length).ConfigureAwait(false);
                        await networkStream.WriteAsync(buffer: buffer, offset: 0, count: buffer.Length).ConfigureAwait(false);
                        dataLength -= nbytes;
                    }
                }

                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public static byte[] ReceiveData(NetworkStream networkStream, int dataLength)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer;
                int nbytes = 0;

                while (dataLength > 0)
                {
                    buffer = new byte[Consts.SIZE_MAX < dataLength ? Consts.SIZE_MAX : dataLength];
                    nbytes = networkStream.Read(buffer: buffer, offset: 0, size: buffer.Length);
                    memoryStream.Write(buffer: buffer, offset: 0, count: buffer.Length);
                    dataLength -= nbytes;
                }

                return memoryStream.ToArray();
            }
        }

        async public static Task<byte[]> ReceiveDataAsync(NetworkStream networkStream, int dataLength)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer;
                int nbytes = 0;

                while (dataLength > 0)
                {
                    buffer = new byte[Consts.SIZE_MAX < dataLength ? Consts.SIZE_MAX : dataLength];
                    nbytes = await networkStream.ReadAsync(buffer: buffer, offset: 0, count: buffer.Length).ConfigureAwait(false);
                    await memoryStream.WriteAsync(buffer: buffer, offset: 0, count: buffer.Length).ConfigureAwait(false);
                    dataLength -= nbytes;
                }

                return memoryStream.ToArray();
            }
        }
    }
}
