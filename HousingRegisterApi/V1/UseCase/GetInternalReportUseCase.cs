using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.Report;
using HousingRegisterApi.V1.Domain.Report.Internal;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetInternalReportUseCase : IGetInternalReportUseCase
    {
        private readonly ILogger<GetInternalReportUseCase> _logger;
        private readonly IFileGateway _fileGateway;
        private readonly IApplicationApiGateway _gateway;
        private readonly IActivityHistory _activityGateway;
        private readonly ICsvService _csvService;

        public GetInternalReportUseCase(
            ILogger<GetInternalReportUseCase> logger,
            IApplicationApiGateway gateway,
            IFileGateway fileGateway,
            IActivityHistory activityGateway,
            ICsvService csvService)
        {
            _logger = logger;
            _gateway = gateway;
            _fileGateway = fileGateway;
            _activityGateway = activityGateway;
            _csvService = csvService;
        }

        public async Task<ExportFile> Execute(InternalReportRequest request)
        {
            _logger.LogInformation($"Attempting to generate report {request.ReportType}");

            var periodStart = new DateTime(request.StartDate.Year, request.StartDate.Month, request.StartDate.Day, 23, 59, 59);
            var periodEnd = new DateTime(request.EndDate.Year, request.EndDate.Month, request.EndDate.Day, 0, 0, 0);
            var file = await GetReportFile(request, periodStart, periodEnd).ConfigureAwait(false);

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

        private async Task<ExportFile> GetReportFile(InternalReportRequest request, DateTime periodStart, DateTime periodEnd)
        {
            return (request.ReportType) switch
            {
                InternalReportType.CasesReport => await GetCaseReport(periodStart, periodEnd).ConfigureAwait(false),
                InternalReportType.PeopleReport => await GetPeopleReport(periodStart, periodEnd).ConfigureAwait(false),
                InternalReportType.CaseActivityReport => await GetCaseActivityReport(periodStart, periodEnd).ConfigureAwait(false),
                _ => null
            };
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

        private async Task<ExportFile> GetPeopleReport(DateTime startDate, DateTime endDate)
        {
            var applications = _gateway.GetApplications(new SearchQueryParameter());

            var applicationsInRange = applications
                .Where(x => x.CreatedAt >= startDate
                        && x.CreatedAt <= endDate).ToList();

            string fileName = $"LBH-PEOPLE REPORT-{DateTime.UtcNow:ddMMyyyy}.csv";

            var exportDataSet = new List<PeopleReportDataRow>();

            applicationsInRange.ForEach(x =>
            {
                var allApplicants = new List<Applicant> { x.MainApplicant };
                allApplicants.AddRange(x.OtherMembers);
                allApplicants.ForEach(a =>
                {
                    if (a.Person != null)
                    {
                        exportDataSet.Add(new PeopleReportDataRow(a, x));
                    }
                });
            });

            var bytes = await _csvService.Generate(exportDataSet.ToArray()).ConfigureAwait(false);
            var file = new ExportFile(fileName, "text/csv", bytes);
            return file;
        }

        private async Task<ExportFile> GetCaseActivityReport(DateTime startDate, DateTime endDate)
        {
            var applications = _gateway.GetApplications(new SearchQueryParameter());

            var applicationsInRange = applications
                .Where(x => x.CreatedAt >= startDate
                        && x.CreatedAt <= endDate).ToList();

            string fileName = $"LBH-CASE-ACTIVITY REPORT-{DateTime.UtcNow:ddMMyyyy}.csv";

            var exportDataSet = new List<CaseActivityReportDataRow>();

            if (applicationsInRange.Any())
            {
                foreach (var application in applicationsInRange)
                {
                    var activities = await _activityGateway.GetActivites(application.Id).ConfigureAwait(false);

                    activities?.ToList().ForEach(a =>
                    {
                        exportDataSet.Add(new CaseActivityReportDataRow(a, application));
                    });
                }
            }

            var bytes = await _csvService.Generate(exportDataSet.ToArray()).ConfigureAwait(false);
            var file = new ExportFile(fileName, "text/csv", bytes);
            return file;
        }
    }
}
