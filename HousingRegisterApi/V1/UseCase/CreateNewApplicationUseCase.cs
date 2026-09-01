using Hackney.Core.Http;
using Hackney.Core.JWT;
using HousingRegisterApi.V1;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Boundary.Response.Exceptions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace HousingRegisterApi.V1.UseCase
{
    public class CreateNewApplicationUseCase : ICreateNewApplicationUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly IActivityGateway _activityGateway;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHttpContextWrapper _contextWrapper;
        private readonly ITokenFactory _tokenFactory;
        private readonly ApiOptions _apiOptions;

        public CreateNewApplicationUseCase(
            IApplicationApiGateway gateway,
            IActivityGateway activityGateway,
            IHttpContextAccessor contextAccessor,
            IHttpContextWrapper contextWrapper,
            ITokenFactory tokenFactory,
            ApiOptions apiOptions)
        {
            _gateway = gateway;
            _activityGateway = activityGateway;
            _contextAccessor = contextAccessor;
            _contextWrapper = contextWrapper;
            _tokenFactory = tokenFactory;
            _apiOptions = apiOptions;
        }

        public ApplicationResponse Execute(CreateApplicationRequest request)
        {
            StaffAuthValidator.EnsureCanCreateApplication(
                _contextAccessor.HttpContext,
                _contextWrapper,
                _tokenFactory,
                _apiOptions);

            if (request.MainApplicant?.ContactInformation != null)
            {
                var email = EmailNormalizer.Normalize(request.MainApplicant.ContactInformation.EmailAddress);
                request.MainApplicant.ContactInformation.EmailAddress = email;

                if (email != null)
                {
                    var existingApplications = _gateway.GetApplicationsByEmail(email).ToList();
                    if (existingApplications.Any())
                    {
                        throw new DuplicateApplicationEmailException(existingApplications.Select(x => x.Id));
                    }
                }
            }

            Application application = _gateway.CreateNewApplication(request);

            var activity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.Created,
                        "", null, application);

            activity.AddChange("", null, application);

            _activityGateway.LogActivity(application, activity);

            return application.ToResponse();
        }
    }
}
