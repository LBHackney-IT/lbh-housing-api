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
        private readonly string _ftpUsername = Environment.GetEnvironmentVariable("FtpUsername");
        private readonly string _ftpPassword = Environment.GetEnvironmentVariable("FtpPassword");
        private readonly string _ftpAddress = Environment.GetEnvironmentVariable("FtpAddress");
        //private readonly string _folderName = Environment.GetEnvironmentVariable("FtpFolder");
        private readonly string _ftpPort = Environment.GetEnvironmentVariable("FtpPort");

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

            byte[] csvFile = data; // Function returns byte[] csv file

            using (var client = new SftpClient(host, port, username, password))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    _logger.LogInformation("Connected to the client");

                    try
                    {
                        var ftpStream = request.GetRequestStream();
                        ftpStream.Write(data, 0, data.Length);
                        ftpStream.Close();
                        return true;
                    }
                    catch (WebException e)
                    {
                        String status = ((FtpWebResponse) e.Response).StatusDescription;
                        _logger.LogError("Unable to upload file to ftp: " + status);
                        return false;
                    }
                }

            }
        }
    }
}
