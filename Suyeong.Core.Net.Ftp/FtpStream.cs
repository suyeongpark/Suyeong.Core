using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Suyeong.Core.Net.Ftp
{
    public static class FtpStream
    {
        public static byte[] FtpDownload(string ftpPath, string user, string password)
        {
            byte[] result = null;

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(requestUriString: ftpPath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(userName: user, password: password);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    result = memoryStream.ToArray();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        async public static Task<byte[]> FtpDownloadAsync(string ftpPath, string user, string password)
        {
            byte[] result = null;

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(requestUriString: ftpPath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(userName: user, password: password);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    result = memoryStream.ToArray();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public static string FtpUpload(byte[] data, string ftpPath, string user, string password)
        {
            string result = string.Empty;

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(requestUriString: ftpPath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(userName: user, password: password);
                request.ContentLength = data.Length;

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(buffer: data, offset: 0, count: data.Length);

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        result = response.StatusDescription;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        async public static Task<string> FtpUploadAsync(byte[] data, string ftpPath, string user, string password)
        {
            string result = string.Empty;

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(requestUriString: ftpPath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(userName: user, password: password);
                request.ContentLength = data.Length;

                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(buffer: data, offset: 0, count: data.Length);

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        result = response.StatusDescription;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
    }
}
