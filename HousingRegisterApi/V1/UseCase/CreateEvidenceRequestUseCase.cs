using Hackney.Core.Http;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Boundary.Response.Exceptions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class CreateEvidenceRequestUseCase : ICreateEvidenceRequestUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly IEvidenceApiGateway _evidenceApiGateway;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHttpContextWrapper _contextWrapper;
        private readonly ITokenGenerator _tokenGenerator;

        public CreateEvidenceRequestUseCase(
            IApplicationApiGateway gateway,
            IEvidenceApiGateway evidenceApiGateway,
            IHttpContextAccessor contextAccessor,
            IHttpContextWrapper contextWrapper,
            ITokenGenerator tokenGenerator)
        {
            _gateway = gateway;
            _evidenceApiGateway = evidenceApiGateway;
            _contextAccessor = contextAccessor;
            _contextWrapper = contextWrapper;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<List<EvidenceRequestResponse>> ExecuteAsync(Guid id, CreateEvidenceRequestBase request)
        {
            // Evidence emails the resident, so only the resident of this application may trigger it.
            // Checked before the application is loaded, so a caller learns nothing about it.
            if (!ResidentAuthValidator.IsResidentForApplication(_contextAccessor.HttpContext, _contextWrapper, _tokenGenerator, id))
            {
                throw new UnauthorizedResidentException();
            }

            var application = _gateway.GetApplicationById(id);
            if (application == null || !request.DocumentTypes.Any())
            {
                return null;
            }

            // Residents request evidence as they submit. Before that the application is still
            // being completed, and afterwards (cancelled, disqualified, assessed) the resident
            // must not be emailed a request for documents.
            if (application.Status != ApplicationStatus.Submitted)
            {
                throw new EvidenceRequestNotAllowedException(application.Status);
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
