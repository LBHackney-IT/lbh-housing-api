using HousingRegisterApi.V1.Domain.Report;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface IListNovaletExportFilesUseCase
    {
        Task<List<ExportFileItem>> Execute();
    }
}
