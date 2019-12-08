using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Suyeong.Core.Util
{
    public static class StreamUtil
    {
        public static byte[] SerializeObject(object data)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, data);

                return memoryStream.ToArray();
            }
        }

        public static object DeserializeObject(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(data, 0, data.Length);
                memoryStream.Position = 0;

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(memoryStream);
            }
        }

        public static byte[] Compress(byte[] data)
        {
            MemoryStream outputStream = new MemoryStream();

            using (DeflateStream deflateStream = new DeflateStream(outputStream, CompressionMode.Compress))
            {
                deflateStream.Write(data, 0, data.Length);
            }

            return outputStream.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            using (MemoryStream outputStream = new MemoryStream())
            using (MemoryStream inputStream = new MemoryStream(data))
            using (DeflateStream deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress))
            {
                deflateStream.CopyTo(outputStream);
                return outputStream.ToArray();
            }
        }

        async public static Task<byte[]> CompressAsync(byte[] data)
        {
            MemoryStream outputStream = new MemoryStream();

            using (DeflateStream deflateStream = new DeflateStream(outputStream, CompressionMode.Compress))
            {
                await deflateStream.WriteAsync(data, 0, data.Length);
            }

            return outputStream.ToArray();
        }

        async public static Task<byte[]> DecompressAsync(byte[] data)
        {
            using (MemoryStream outputStream = new MemoryStream())
            using (MemoryStream inputStream = new MemoryStream(data))
            using (DeflateStream deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress))
            {
                await deflateStream.CopyToAsync(outputStream);
                return outputStream.ToArray();
            }
        }

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (Aes algorithm = Aes.Create())
            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(key, iv))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
            }

            return memoryStream.ToArray();
        }

        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (MemoryStream outputStream = new MemoryStream())
            using (MemoryStream inputStream = new MemoryStream(data))
            using (Aes algorithm = Aes.Create())
            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv))
            using (CryptoStream cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
            {
                cryptoStream.CopyTo(outputStream);
                return outputStream.ToArray();
            }
        }

        async public static Task<byte[]> EncryptAsync(byte[] data, byte[] key, byte[] iv)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (Aes algorithm = Aes.Create())
            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(key, iv))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                await cryptoStream.WriteAsync(data, 0, data.Length);
            }

            return memoryStream.ToArray();
        }

        async public static Task<byte[]> DecryptAsync(byte[] data, byte[] key, byte[] iv)
        {
            using (MemoryStream outputStream = new MemoryStream())
            using (MemoryStream inputStream = new MemoryStream(data))
            using (Aes algorithm = Aes.Create())
            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv))
            using (CryptoStream cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
            {
                await cryptoStream.CopyToAsync(outputStream);
                return outputStream.ToArray();
            }
        }

        public static byte[] EncryptWithCompress(byte[] data, byte[] key, byte[] iv)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (Aes algorithm = Aes.Create())
            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(key, iv))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            using (DeflateStream deflateStream = new DeflateStream(cryptoStream, CompressionMode.Compress))
            {
                deflateStream.Write(data, 0, data.Length);
            }

            return memoryStream.ToArray();
        }

        public static byte[] DecryptWithDecompress(byte[] data, byte[] key, byte[] iv)
        {
            using (MemoryStream outputStream = new MemoryStream())
            using (MemoryStream inputStream = new MemoryStream(data))
            using (Aes algorithm = Aes.Create())
            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv))
            using (CryptoStream cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
            using (DeflateStream deflateStream = new DeflateStream(cryptoStream, CompressionMode.Decompress))
            {
                deflateStream.CopyTo(outputStream);
                return outputStream.ToArray();
            }
        }

        async public static Task<byte[]> EncryptWithCompressAsync(byte[] data, byte[] key, byte[] iv)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (Aes algorithm = Aes.Create())
            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(key, iv))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            using (DeflateStream deflateStream = new DeflateStream(cryptoStream, CompressionMode.Compress))
            {
                await deflateStream.WriteAsync(data, 0, data.Length);
            }

            return memoryStream.ToArray();
        }

        async public static Task<byte[]> DecryptWithDecompressAsync(byte[] data, byte[] key, byte[] iv)
        {
            using (MemoryStream outputStream = new MemoryStream())
            using (MemoryStream inputStream = new MemoryStream(data))
            using (Aes algorithm = Aes.Create())
            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv))
            using (CryptoStream cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
            using (DeflateStream deflateStream = new DeflateStream(cryptoStream, CompressionMode.Decompress))
            {
                await deflateStream.CopyToAsync(outputStream);
                return outputStream.ToArray();
            }
        }
    }
}
