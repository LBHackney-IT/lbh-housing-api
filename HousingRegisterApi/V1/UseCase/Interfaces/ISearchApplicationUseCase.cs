using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface ISearchApplicationUseCase
    {
        Task<ApplicationSearchPagedResponse> Execute(ApplicationSearchRequest request);
    }
}
