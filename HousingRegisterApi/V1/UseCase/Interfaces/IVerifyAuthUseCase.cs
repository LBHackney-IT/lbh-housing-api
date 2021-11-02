using System;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface IVerifyAuthUseCase
    {
        VerifyAuthResponse Execute(VerifyAuthRequest request);
    }
}
