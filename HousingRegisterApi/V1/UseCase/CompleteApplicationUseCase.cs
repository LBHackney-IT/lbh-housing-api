using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System;

namespace HousingRegisterApi.V1.UseCase
{
    public class CompleteApplicationUseCase : ICompleteApplicationUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public CompleteApplicationUseCase(IApplicationApiGateway gateway, ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _gateway = gateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public ApplicationResponse Execute(Guid id)
        {
            var application = _gateway.CompleteApplication(id);

            _snsGateway.Publish(_snsFactory.Create(application, null));

            return application.ToResponse();
        }
    }
}
