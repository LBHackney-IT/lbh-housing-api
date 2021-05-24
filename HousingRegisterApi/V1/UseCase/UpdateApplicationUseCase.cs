using System;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class UpdateApplicationUseCase : IUpdateApplicationUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        public UpdateApplicationUseCase(IApplicationApiGateway gateway)
        {
            _gateway = gateway;
        }

        public ApplicationResponse Execute(Guid id, UpdateApplicationRequest request)
        {
            return _gateway.UpdateApplication(id, request).ToResponse();
        }
    }
}
