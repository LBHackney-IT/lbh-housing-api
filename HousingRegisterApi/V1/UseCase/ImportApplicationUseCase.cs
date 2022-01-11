using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class ImportApplicationUseCase : IImportApplicationUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        public ImportApplicationUseCase(IApplicationApiGateway gateway)
        {
            _gateway = gateway;
        }

        public ApplicationResponse Execute(ImportApplicationRequest request)
        {
            return _gateway.ImportApplication(request).ToResponse();
        }
    }
}
