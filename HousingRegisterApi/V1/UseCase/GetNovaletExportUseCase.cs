using HousingRegisterApi.V1.Domain.FileExport;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetNovaletExportUseCase : IGetNovaletExportUseCase
    {
        private readonly ILogger _logger;
        private readonly IApplicationApiGateway _gateway;
        private readonly ICSVService _csvService;

        public GetNovaletExportUseCase(
            ILogger<GetNovaletExportUseCase> logger,
            IApplicationApiGateway gateway,
            ICSVService csvService)
        {
            _logger = logger;
            _gateway = gateway;
            _csvService = csvService;
        }

        public async Task<FileExportResult> Execute(Guid id)
        {
            var application = _gateway.GetApplicationById(id);
            string fileName = $"LBH-APPLICANT FEED-{DateTime.UtcNow.ToString("yyyyMMdd")}";

            if (application == null)
            {
                return null;
            }

            var exportDataRow = new NovaletExportDataRow(application);
            var bytes = await _csvService.Generate(exportDataRow, new CsvParameters()
            {
                IncludeHeaders = true
            }).ConfigureAwait(false);

            return new FileExportResult(fileName, "text/csv", bytes); ;
        }
    }
}
