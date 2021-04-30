using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System;

namespace HousingRegisterApi.V1.UseCase
{    
    public class GetApplicationByIdUseCase : IGetApplicationByIdUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        public GetApplicationByIdUseCase(IApplicationApiGateway gateway)
        {
            _gateway = gateway;
        }
        
        public ApplicationResponse Execute(Guid id)
        {
            return _gateway.GetApplicationById(id).ToResponse();
        }
    }
}
