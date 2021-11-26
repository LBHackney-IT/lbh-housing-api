using HousingRegisterApi.V1.Boundary.Response;
using System;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface IViewingApplicationUseCase
    {
        ApplicationResponse Execute(Guid id);
    }
}
