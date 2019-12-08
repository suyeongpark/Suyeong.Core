using System.Security.Cryptography;
using System.Text;

namespace Suyeong.Core.Util
{
    public static class CryptUtil
    {
        public static string GetChecksumByMD5(byte[] file)
        {
            MD5 md5 = MD5.Create();

            byte[] hash = md5.ComputeHash(buffer: file);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hash)
            {
                // 16진수로 변환
                sb.Append(value: b.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
