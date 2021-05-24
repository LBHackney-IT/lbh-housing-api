using System;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface IUpdateApplicationUseCase
    {
        ApplicationResponse Execute(Guid id, UpdateApplicationRequest request);
    }
}
