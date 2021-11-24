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

        public async Task<bool> Execute(string fileName)
        {
            await _fileGateway.UpdateAttributes(fileName, new Dictionary<string, string> {
                { "status", "approved" }
            }).ConfigureAwait(false);

            return true;
        }
    }
}
