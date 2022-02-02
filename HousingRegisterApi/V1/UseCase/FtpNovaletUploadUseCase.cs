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

        public async Task<ExportFile> Execute()
        {
            string fileName = $"LBH-APPLICANT FEED-{DateTime.UtcNow:ddMMyyyy}.csv";

            _logger.LogInformation($"Attempting to generate {fileName}");

        }
    }
}
