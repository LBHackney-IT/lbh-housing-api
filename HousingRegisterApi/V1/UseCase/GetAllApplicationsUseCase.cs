using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetAllApplicationsUseCase : IGetAllApplicationsUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        public GetAllApplicationsUseCase(IApplicationApiGateway gateway)
        {
            _gateway = gateway;
        }

        public ApplicationList Execute()
        {
            return new ApplicationList { Results = _gateway.GetAll().ToResponse() };
        }
    }
}
