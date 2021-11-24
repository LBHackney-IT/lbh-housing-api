using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.FileExport;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetNovaletExportUseCase : IGetNovaletExportUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly ICsvService _csvService;

        public GetNovaletExportUseCase(
            IApplicationApiGateway gateway,
            ICsvService csvService)
        {
            _gateway = gateway;
            _csvService = csvService;
        }

        public async Task<ExportFile> Execute()
        {
            var applications = _gateway.GetApplicationsAtStatus(ApplicationStatus.Active);

            string fileName = $"LBH-APPLICANT FEED-{DateTime.UtcNow:ddMMyyyy}.csv";

            if (!applications.Any())
            {
                return null;
            }

            var exportDataSet = applications.Select(x => new NovaletExportDataRow(x)).ToArray();
            var bytes = await _csvService.Generate(exportDataSet).ConfigureAwait(false);

            return new ExportFile(fileName, "text/csv", bytes); ;
        }
    }
}
