using HousingRegisterApi.V1.Domain.Report;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class ListNovaletExportFilesUseCase : IListNovaletExportFilesUseCase
    {
        private readonly IFileGateway _fileGateway;

        public ListNovaletExportFilesUseCase(IFileGateway fileGateway)
        {
            _fileGateway = fileGateway;
        }

        public async Task<List<ExportFileItem>> Execute(int numberToReturn = 1000)
        {
            return await _fileGateway.ListFiles("Novalet", numberToReturn).ConfigureAwait(false);
        }
    }
}
