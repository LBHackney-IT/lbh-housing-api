using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.FileExport;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetNovaletExportUseCase : IGetNovaletExportUseCase
    {
        private readonly ILogger _logger;
        private readonly IApplicationApiGateway _gateway;
        private readonly ICsvService _csvService;

        public GetNovaletExportUseCase(
            ILogger<GetNovaletExportUseCase> logger,
            IApplicationApiGateway gateway,
            ICsvService csvService)
        {
            _logger = logger;
            _gateway = gateway;
            _csvService = csvService;
        }

        public async Task<FileExportResult> Execute()
        {
            var applications = _gateway.GetApplicationsAtStatus(ApplicationStatus.Active);
            string fileName = $"LBH-APPLICANT FEED-{DateTime.UtcNow.ToString("ddMMyyyy")}";

            if (!applications.Any())
            {
                return null;
            }

            var exportDataSet = applications.Select(x => new NovaletExportDataRow(x)).ToArray();
            var bytes = await _csvService.Generate(exportDataSet).ConfigureAwait(false);

            return new FileExportResult(fileName, "text/csv", bytes); ;
        }
    }
}
