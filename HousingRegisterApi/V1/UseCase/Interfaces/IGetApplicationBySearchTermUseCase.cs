using HousingRegisterApi.V1.Boundary.Response;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface IGetApplicationBySearchTermUseCase
    {
        ApplicationList Execute(string searchTerm);
    }
}
