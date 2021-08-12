using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using System;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface ICompleteApplicationUseCase
    {
        ApplicationResponse Execute(Guid id);
    }
}
