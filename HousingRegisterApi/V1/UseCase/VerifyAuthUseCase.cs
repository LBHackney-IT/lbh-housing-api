using System;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class VerifyAuthUseCase : IVerifyAuthUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        public VerifyAuthUseCase(IApplicationApiGateway gateway)
        {
            _gateway = gateway;
        }

        public VerifyAuthResponse Execute(Guid id, VerifyAuthRequest request)
        {
            var response = _gateway.ConfirmVerifyCode(id, request);
            return response != null ? new VerifyAuthResponse() { Success = true } : null;
        }
    }
}
