using HousingRegisterApi.V1.Domain.Report;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface ICreateNovaletExportUseCase
    {
        Task<ExportFile> Execute();
    }
}
