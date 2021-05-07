using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class CreateNewApplicationUseCase : ICreateNewApplicationUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        public CreateNewApplicationUseCase(IApplicationApiGateway gateway)
        {
            _gateway = gateway;
        }

        public ApplicationResponse Execute(CreateApplicationRequest request)
        {
            return _gateway.CreateNewApplication(request).ToResponse();
        }
    }
}
