using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class ImportApplicationUseCase : IImportApplicationUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly IActivityGateway _activityGateway;

        public ImportApplicationUseCase(
            IApplicationApiGateway gateway,
            IActivityGateway applicationHistory)
        {
            _gateway = gateway;
            _activityGateway = applicationHistory;
        }

        public ApplicationResponse Execute(ImportApplicationRequest request)
        {

            var application = _gateway.ImportApplication(request);

            _activityGateway.LogActivity(application, new EntityActivity<ApplicationActivityType>(ApplicationActivityType.ImportedFromLegacyDatabase));

            return application.ToResponse();
        }
    }
}
