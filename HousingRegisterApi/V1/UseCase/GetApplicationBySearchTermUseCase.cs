using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetApplicationBySearchTermUseCase : IGetApplicationBySearchTermUseCase
    {
        private readonly IApplicationApiGateway _gateway;

        public GetApplicationBySearchTermUseCase(IApplicationApiGateway gateway)
        {
            _gateway = gateway;
        }

        public ApplicationList Execute(string searchTerm)
        {
            return new ApplicationList { Results = _gateway.GetAllBySearchTerm(searchTerm).ToResponse() };
        }
    }
}
