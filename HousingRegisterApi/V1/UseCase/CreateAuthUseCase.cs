using System;
using System.Collections.Generic;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class CreateAuthUseCase : ICreateAuthUseCase
    {
        private readonly IApplicationApiGateway _applicationGateway;
        private readonly INotifyGateway _notifyGateway;
        private readonly IActivityGateway _activityGateway;

        public CreateAuthUseCase(
            IApplicationApiGateway applicationGateway,
            INotifyGateway notifyGateway,
            IActivityGateway activityGateway)
        {
            _applicationGateway = applicationGateway;
            _notifyGateway = notifyGateway;
            _activityGateway = activityGateway;
        }

        public CreateAuthResponse Execute(CreateAuthRequest request)
        {
            // check if an uncompleted application exists
            // if so, use that, or create a blank one
            var incompleteApplication = _applicationGateway.GetIncompleteApplication(request.Email);
            var applicationId = incompleteApplication?.Id;

            if (incompleteApplication == null)
            {
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

                activity.AddChange("", null, blankApplication);

                _activityGateway.LogActivity(blankApplication, activity);

            }

            // this generates a new verification code and assigns it to the application entity
            var application = _applicationGateway.CreateVerifyCode(applicationId.Value, request);
            if (application == null)
            {
                return null;
            }

            var notifyResponse = _notifyGateway.SendVerifyCode(application.MainApplicant, application.VerifyCode);
            return new CreateAuthResponse()
            {
                Success = notifyResponse != null
            };
        }
    }
}
