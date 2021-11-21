using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class ApproveNovaletExportUseCase : IApproveNovaletExportUseCase
    {
        private readonly IFileGateway _fileGateway;

        public ApproveNovaletExportUseCase(IFileGateway fileGateway)
        {
            _fileGateway = fileGateway;
        }

        public async Task Execute(string fileName)
        {
            var fileMeta = new Dictionary<string, string>()
            {
                {"ApprovedForExport", "true"}
            };

            await _fileGateway.UpdateMetadata(fileName, fileMeta).ConfigureAwait(false);
        }
    }
}
