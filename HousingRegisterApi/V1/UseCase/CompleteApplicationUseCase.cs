using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Gateways.Interfaces;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System;

namespace HousingRegisterApi.V1.UseCase
{
    public class CompleteApplicationUseCase : ICompleteApplicationUseCase
    {
        private readonly IAuditHistory _auditHistory;
        private readonly IApplicationApiGateway _gateway;

        public CompleteApplicationUseCase(
            IAuditHistory auditHistory,
            IApplicationApiGateway gateway)
        {
            _auditHistory = auditHistory;
            _gateway = gateway;
        }

        public ApplicationResponse Execute(Guid id)
        {
            var application = _gateway.CompleteApplication(id);

            _auditHistory.Audit(application);

            return application.ToResponse();
        }
    }
}
