using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class FtpHelper : IFtpHelper
    {
        private readonly string _ftpUsername = Environment.GetEnvironmentVariable("NOVALET_FTP_USERNAME");
        private readonly string _ftpPassword = Environment.GetEnvironmentVariable("NOVALET_FTP_PASSWORD");
        private readonly string _ftpAddress = Environment.GetEnvironmentVariable("NOVALET_FTP_ADDRESS");
        private readonly string _ftpPort = Environment.GetEnvironmentVariable("NOVALET_FTP_PORT");

        private readonly ILogger<FtpHelper> _logger;

        public FtpHelper(ILogger<FtpHelper> logger)
        {
            _logger = logger;
        }

        public bool UploadDataToFtp(byte[] data, string fileName)
        {
            var host = _ftpAddress;
            var port = 22;
            int portInt;
            bool parsedInt = int.TryParse(_ftpPort, out portInt);
            if (parsedInt)
            {
                port = portInt;
            }
            var username = _ftpUsername;
            var password = _ftpPassword;

            using (var client = new SftpClient(host, port, username, password))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    _logger.LogInformation("Connected to the client");

                    using (var ms = new MemoryStream(data))
                    {
                        client.BufferSize = (uint) ms.Length; // bypass Payload error large files
                        client.UploadFile(ms, fileName);
                        return true;
                    }
                }
                else
                {
                    _logger.LogError("Couldn't connect");
                    return false;
                }
            }
        }
    }
}
