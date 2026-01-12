using System;
using System.Collections.Generic;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using Hackney.Core.Logging;

namespace HousingRegisterApi.V1.UseCase
{
    public class CreateAuthUseCase : ICreateAuthUseCase
    {
        private readonly ILogger _logger;
        private readonly IApplicationApiGateway _applicationGateway;
        private readonly INotifyGateway _notifyGateway;
        private readonly IActivityGateway _activityGateway;

        public CreateAuthUseCase(
            ILogger<CreateAuthUseCase> logger,
            IApplicationApiGateway applicationGateway,
            INotifyGateway notifyGateway,
            IActivityGateway activityGateway)
        {
            _logger = logger;
            _applicationGateway = applicationGateway;
            _notifyGateway = notifyGateway;
            _activityGateway = activityGateway;
        }

        [LogCall]
        public CreateAuthResponse Execute(CreateAuthRequest request)
        {
            // check if an uncompleted application exists
            // if so, use that, or create a blank one
            var incompleteApplication = _applicationGateway.GetIncompleteApplication(request.Email);
            var applicationId = incompleteApplication?.Id;

            if (incompleteApplication == null)
            {
                _logger.LogInformation($"Creating new application for email: {request.Email}");
                var blankApplication = _applicationGateway.CreateNewApplication(new CreateApplicationRequest()
                {
                    MainApplicant = new Applicant()
                    {
                        ContactInformation = new ContactInformation()
                        {
                            EmailAddress = request.Email
                        }
                    },
                    OtherMembers = new List<Applicant>(),
                    Status = ApplicationStatus.Verification
                });

                applicationId = blankApplication.Id;


                var activity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.Created,
                        "", null, blankApplication);

                _logger.LogInformation($"Adding activity for application ID: {applicationId}");
                activity.AddChange("", null, blankApplication);

                _activityGateway.LogActivity(blankApplication, activity);
            } else {
                _logger.LogInformation($"Using existing incomplete application ID: {applicationId} for email: {request.Email}");
            }

            // this generates a new verification code and assigns it to the application entity
            _logger.LogInformation($"Creating verification code for application ID: {applicationId}");
            var application = _applicationGateway.CreateVerifyCode(applicationId.Value, request);
            if (application == null)
            {
                throw new Exception($"Failed to create verification code for application ID: {applicationId}");
            }

            _logger.LogInformation($"Sending verification code for application ID: {applicationId}");
            var notifyResponse = _notifyGateway.SendVerifyCode(application.MainApplicant, application.VerifyCode);
            return new CreateAuthResponse()
            {
                Success = notifyResponse != null
            };
        }
    }
}
