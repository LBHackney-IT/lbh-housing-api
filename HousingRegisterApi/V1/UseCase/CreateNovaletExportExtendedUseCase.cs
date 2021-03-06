using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.Report;
using HousingRegisterApi.V1.Domain.Report.Novalet;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class CreateNovaletExportExtendedUseCase : ICreateNovaletExportExtendedUseCase
    {
        private readonly ILogger<CreateNovaletExportUseCase> _logger;
        private readonly IFileGateway _fileGateway;
        private readonly IApplicationApiGateway _gateway;
        private readonly ICsvService _csvService;

        public CreateNovaletExportExtendedUseCase(
            ILogger<CreateNovaletExportUseCase> logger,
            IApplicationApiGateway gateway,
            IFileGateway fileGateway,
            ICsvService csvService)
        {
            _logger = logger;
            _gateway = gateway;
            _fileGateway = fileGateway;
            _csvService = csvService;
        }

        public async Task<ExportFile> Execute()
        {
            var applications = _gateway.GetApplicationsAtStatus(ApplicationStatus.Active);
            string fileName = $"LBH-APPLICANT FEED-WITH-URL-{DateTime.UtcNow:yyyyMMdd}.csv";

            if (!applications.Any())
            {
                _logger.LogInformation($"No export file was generated this time");
                return null;
            }

            _logger.LogInformation($"Attempting to generate {fileName}");

            var exportDataSet = applications.Select(x => new NovaletExportDataRowExtended(x)).ToArray();
            var bytes = await _csvService.Generate(exportDataSet).ConfigureAwait(false);
            var file = new ExportFile(fileName, "text/csv", bytes);

            if (bytes.Length > 0)
            {
                await _fileGateway.SaveFile(file, "Novalet").ConfigureAwait(false);
                _logger.LogInformation($"File {file.FileName} was succesfully generated & has a size of {bytes.Length} bytes");
                return file;
            }
            else
            {
                _logger.LogInformation($"No extended file was generated this time");
                return null;
            }
        }
    }
}
