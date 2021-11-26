using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System;

namespace HousingRegisterApi.V1.UseCase
{
    public class ViewingApplicationUseCase : IViewingApplicationUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly IActivityHistory _applicationHistory;

        public ViewingApplicationUseCase(
            IApplicationApiGateway gateway,
            IActivityHistory applicationHistory)
        {
            _gateway = gateway;
            _applicationHistory = applicationHistory;
        }

        public ApplicationResponse Execute(Guid id)
        {
            var application = _gateway.GetApplicationById(id);

            if (application != null)
            {
                _applicationHistory.LogActivity(application, new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseViewedByUser));
                return application.ToResponse();
            }

            return null;
        }
    }
}
