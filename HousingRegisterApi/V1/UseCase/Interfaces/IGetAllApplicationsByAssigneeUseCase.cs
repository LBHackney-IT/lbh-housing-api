using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface IGetAllApplicationsByAssigneeUseCase
    {
        Task<ApplicationSearchPagedResponse> Execute(SearchQueryParameter searchParameters);
    }
}
