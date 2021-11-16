using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetApplicationByIdUseCase : IGetApplicationByIdUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly IActivityHistory _applicationHistory;

        public GetApplicationByIdUseCase(
            IApplicationApiGateway gateway,
            IActivityHistory applicationHistory)
        {
            _gateway = gateway;
            _applicationHistory = applicationHistory;
        }

        public ApplicationResponse Execute(Guid id)
        {
            var applicationResponse = _gateway.GetApplicationById(id).ToResponse();

            _applicationHistory.LogActivity(id, new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseViewed));

            return applicationResponse;
        }
    }
}
