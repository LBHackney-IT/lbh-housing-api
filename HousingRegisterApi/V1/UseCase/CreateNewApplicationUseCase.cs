using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class CreateNewApplicationUseCase : ICreateNewApplicationUseCase
    {
        private readonly IApplicationApiGateway _gateway;

        private readonly IActivityGateway _activityGateway;
        public CreateNewApplicationUseCase(IApplicationApiGateway gateway, IActivityGateway activityGateway)
        {
            _gateway = gateway;
            _activityGateway = activityGateway;
        }

        public ApplicationResponse Execute(CreateApplicationRequest request)
        {
            var application = _gateway.CreateNewApplication(request).ToResponse();

            var activity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.Created,
                        "", request, request.Status);

            activity.AddChange("", null, application);

            return application;
        }
    }
}
