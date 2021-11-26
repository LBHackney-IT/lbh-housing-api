using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface IApproveNovaletExportUseCase
    {
        Task<bool> Execute(string fileName);
    }
}
