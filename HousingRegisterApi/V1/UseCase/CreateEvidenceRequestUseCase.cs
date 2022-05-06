using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class CreateEvidenceRequestUseCase : ICreateEvidenceRequestUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly IEvidenceApiGateway _evidenceApiGateway;

        public CreateEvidenceRequestUseCase(IApplicationApiGateway gateway, IEvidenceApiGateway evidenceApiGateway)
        {
            _gateway = gateway;
            _evidenceApiGateway = evidenceApiGateway;
        }

        public async Task<List<EvidenceRequestResponse>> ExecuteAsync(Guid id, CreateEvidenceRequestBase request)
        {
            var application = _gateway.GetApplicationById(id);
            if (application == null || !request.DocumentTypes.Any())
            {
                return null;
            }

            var response = new List<EvidenceRequestResponse>();

            var createEvidenceRequest = BuildEvidenceRequest(application.MainApplicant, request.DocumentTypes, request.UserRequestedBy);
            var evidenceResponse = await _evidenceApiGateway.CreateEvidenceRequest(createEvidenceRequest).ConfigureAwait(true);

            response.Add(evidenceResponse);

            return response;
        }

        private static CreateEvidenceRequest BuildEvidenceRequest(Applicant applicant, List<string> documentTypes, string userRequestedBy)
        {
            return new CreateEvidenceRequest
            {
                Resident = new ResidentRequest
                {
                    Name = applicant.Person.FullName,
                    Email = applicant.ContactInformation.EmailAddress
                },
                DocumentTypes = documentTypes,
                DeliveryMethods = new List<string> { "EMAIL" },
                Team = "Housing Register",
                Reason = "Assessment",
                UserRequestedBy = string.IsNullOrEmpty(userRequestedBy) ? "housingregister@hackney.gov.uk" : userRequestedBy,
                NotificationEmail = "housingregister@hackney.gov.uk"
            };
        }
    }
}
