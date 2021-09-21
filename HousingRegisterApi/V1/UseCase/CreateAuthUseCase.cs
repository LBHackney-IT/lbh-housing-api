using System;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class CreateAuthUseCase : ICreateAuthUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        public CreateAuthUseCase(IApplicationApiGateway gateway)
        {
            _gateway = gateway;
        }

        public CreateAuthResponse Execute(Guid id, CreateAuthRequest request)
        {
            var response = _gateway.CreateVerifyCode(id, request);
            return response != null ? new CreateAuthResponse() { Success = true } : null;
        }
    }
}
