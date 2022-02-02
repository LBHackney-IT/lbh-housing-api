using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.Report;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HousingRegisterApi.V1.Infrastructure;

namespace HousingRegisterApi.V1.UseCase
{
    public class FtpNovaletUploadUseCase : IFtpNovaletUploadUseCase
    {
        private readonly ILogger<FtpNovaletUploadUseCase> _logger;
        private readonly IFileGateway _fileGateway;
        private readonly IFtpHelper _ftpHelper;

        public FtpNovaletUploadUseCase(
            ILogger<FtpNovaletUploadUseCase> logger,
            IFileGateway fileGateway,
            IFtpHelper ftpHelper)
        {
            _logger = logger;
            _fileGateway = fileGateway;
            _ftpHelper = ftpHelper;
        }

        public async Task<bool> Execute()
        {
            string fileName = $"LBH-APPLICANT FEED-{DateTime.UtcNow:ddMMyyyy}.csv";

            _logger.LogInformation($"Attempting to to fetch approved file {fileName}");
            //Get approved file from S3
            var file = await _fileGateway.GetFile(fileName, "Novalet").ConfigureAwait(false);
            if (file.Attributes.ContainsKey("approvedOn"))
            {
                //upload fetch file to Novalet Ftp
                bool result = _ftpHelper.UploadDataToFtp(file.Data, fileName);
                return result;
            }
            else
            {
                _logger.LogInformation($"Approved Novalet file not found");
                return false;
            }
        }
    }
}
