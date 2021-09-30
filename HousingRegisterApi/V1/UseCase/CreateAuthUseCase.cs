using System;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class CreateAuthUseCase : ICreateAuthUseCase
    {
        private readonly IApplicationApiGateway _applicationGateway;
        private readonly INotifyGateway _notifyGateway;

        public CreateAuthUseCase(
            IApplicationApiGateway applicationGateway,
            INotifyGateway notifyGateway)
        {
            _applicationGateway = applicationGateway;
            _notifyGateway = notifyGateway;
        }

        public CreateAuthResponse Execute(Guid id, CreateAuthRequest request)
        {
            var response = _applicationGateway.CreateVerifyCode(id, request);
            if (response == null)
            {
                return null;
            }

            var notifyResponse = _notifyGateway.SendVerifyCode(response.MainApplicant, response.VerifyCode);
            return new CreateAuthResponse()
            {
                Success = notifyResponse != null
            };
        }
    }
}
