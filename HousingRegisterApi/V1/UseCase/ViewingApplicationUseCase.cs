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
        private readonly IActivityGateway _activityGateway;

        public ViewingApplicationUseCase(
            IApplicationApiGateway gateway,
            IActivityGateway applicationHistory)
        {
            _gateway = gateway;
            _activityGateway = applicationHistory;
        }

        public ApplicationResponse Execute(Guid id)
        {
            var application = _gateway.GetApplicationById(id);

            if (application != null)
            {
                _activityGateway.LogActivity(application, new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseViewedByUser));
                return application.ToResponse();
            }

            return null;
        }
    }
}
