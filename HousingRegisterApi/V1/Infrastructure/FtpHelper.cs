using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class FtpHelper : IFtpHelper
    {
        private readonly string _ftpUsername = Environment.GetEnvironmentVariable("NOVALET_FTP_USERNAME");
        private readonly string _ftpPassword = Environment.GetEnvironmentVariable("NOVALET_FTP_PASSWORD");
        private readonly string _ftpAddress = Environment.GetEnvironmentVariable("NOVALET_FTP_ADDRESS");
        private readonly string _folderName = Environment.GetEnvironmentVariable("NOVALET_FTP_FOLDER");
        private readonly string _ftpPort = Environment.GetEnvironmentVariable("NOVALET_FTP_PORT");

        private readonly ILogger<FtpHelper> _logger;

        public FtpHelper(ILogger<FtpHelper> logger)
        {
            _logger = logger;
        }
        public bool UploadDataToFtp(byte[] data, string fileName)
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "ftp";
            uriBuilder.Host = _ftpAddress;
            uriBuilder.Path = _folderName + "/" + fileName;
            int portInt;
            bool parsedInt = int.TryParse(_ftpPort, out portInt);
            if (parsedInt)
            {
                uriBuilder.Port = portInt;
            }
            Uri uri = uriBuilder.Uri;
            var request = (FtpWebRequest) WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = false;
            request.EnableSsl = true;

            request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);

            var ftpStream = request.GetRequestStream();
            ftpStream.Write(data, 0, data.Length);
            ftpStream.Close();
            return true;
        }

    }
}
