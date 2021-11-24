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
        private readonly IFileGateway _fileGateway;

        public GetNovaletExportUseCase(IFileGateway fileGateway)
        {
            _fileGateway = fileGateway;
        }

        public async Task<ExportFile> Execute(string fileName)
        {
            var file = await _fileGateway.GetFile(fileName).ConfigureAwait(false);
            return file;
        }
    }
}
