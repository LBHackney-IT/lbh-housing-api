using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.Report;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetInternalReportUseCase : IGetInternalReportUseCase
    {
        private readonly ILogger<GetInternalReportUseCase> _logger;
        private readonly IFileGateway _fileGateway;
        private readonly IApplicationApiGateway _gateway;
        private readonly ICsvService _csvService;

        public GetInternalReportUseCase(
            ILogger<GetInternalReportUseCase> logger,
            IApplicationApiGateway gateway,
            IFileGateway fileGateway,
            ICsvService csvService)
        {
            _logger = logger;
            _gateway = gateway;
            _fileGateway = fileGateway;
            _csvService = csvService;
        }

        public async Task<ExportFile> Execute(InternalReportRequest request)
        {
            _logger.LogInformation($"Attempting to generate report {request.ReportType}");

            var periodStart = new DateTime(request.StartDate.Year, request.StartDate.Month, request.StartDate.Day, 23, 59, 59);
            var periodEnd = new DateTime(request.EndDate.Year, request.EndDate.Month, request.EndDate.Day, 0, 0, 0);

            var file = await GetCaseReport(periodStart, periodEnd).ConfigureAwait(false);

            if (file != null)
            {
                // save file to s3 gateway
                // var response = _fileGateway.SaveFile(file).ConfigureAwait(false);
                _logger.LogInformation($"File {file.FileName} was succesfully generated");
                return file;
            }
            else
            {
                _logger.LogInformation($"No export file was generated this time");
                return null;
            }
        }

        private async Task<ExportFile> GetCaseReport(DateTime startDate, DateTime endDate)
        {
            var applications = _gateway.GetApplications(new SearchQueryParameter());

            var applicationsInRange = applications
                .Where(x => x.CreatedAt >= startDate
                        && x.CreatedAt <= endDate).ToList();

            string fileName = $"LBH-CASES REPORT-{DateTime.UtcNow:ddMMyyyy}.csv";

            var exportDataSet = applicationsInRange.Select(x => new CasesReportDataRow(x)).ToArray();
            var bytes = await _csvService.Generate(exportDataSet).ConfigureAwait(false);
            var file = new ExportFile(fileName, "text/csv", bytes);
            return file;
        }
    }
}
