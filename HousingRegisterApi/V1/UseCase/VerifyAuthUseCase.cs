using System;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class VerifyAuthUseCase : IVerifyAuthUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly ITokenGenerator _tokenGenerator;

        public VerifyAuthUseCase(IApplicationApiGateway gateway,
            ITokenGenerator tokenGenerator)
        {
            _gateway = gateway;
            _tokenGenerator = tokenGenerator;
        }

        public VerifyAuthResponse Execute(VerifyAuthRequest request)
        {
            var application = _gateway.ConfirmVerifyCode(request);
            if (application == null)
            {
                return null;
            }

            var accessToken = _tokenGenerator.GenerateTokenForApplication(application.Id);
            return new VerifyAuthResponse()
            {
                AccessToken = accessToken
            };
        }
    }
}
