using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Boundary.Response.Exceptions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System;

namespace HousingRegisterApi.V1.UseCase
{
    public class UpdateApplicationUseCase : IUpdateApplicationUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly IBiddingNumberGenerator _biddingNumberGenerator;
        private readonly IActivityHistory _applicationHistory;

        public UpdateApplicationUseCase(
            IApplicationApiGateway gateway,
            IBiddingNumberGenerator biddingNumberGenerator,
            IActivityHistory applicationAudit)
        {
            _gateway = gateway;
            _biddingNumberGenerator = biddingNumberGenerator;
            _applicationHistory = applicationAudit;
        }

        public ApplicationResponse Execute(Guid id, UpdateApplicationRequest request)
        {
            if (request.Assessment != null
                && (request.Assessment.GenerateBiddingNumber || !string.IsNullOrEmpty(request.Assessment.BiddingNumber)))
            {
                var searchParameters = new SearchQueryParameter()
                {
                    HasAssessment = true
                };

                var applications = _gateway.GetApplications(searchParameters);
                if (request.Assessment.GenerateBiddingNumber)
                {
                    var biddingNumber = _biddingNumberGenerator.GetNextBiddingNumber(applications);
                    request.Assessment.BiddingNumber = biddingNumber;
                }
                else if (!string.IsNullOrEmpty(request.Assessment.BiddingNumber)
                    && _biddingNumberGenerator.IsExistingBiddingNumber(applications, id, request.Assessment.BiddingNumber))
                {
                    throw new DuplicateBiddingNumberException("Unable to update application with duplicate bidding number");
                }
            }

            // get list of all update activities prior to updating the application
            var application = _gateway.GetApplicationById(id);
            var activities = GetApplicationActivities(application, request);

            var result = _gateway.UpdateApplication(id, request).ToResponse();
            if (null != result)
            {
                // audit the update
                _applicationHistory.LogUpdate(id, activities);
            }

            return result;
        }

        /// <summary>
        /// Get a collection of all update activites performed on an application
        /// </summary>
        /// <param name="application"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private static EntityActivityCollection<ApplicationActivityType> GetApplicationActivities(Application application, UpdateApplicationRequest request)
        {
            var activities = new EntityActivityCollection<ApplicationActivityType>();

            if (application != null && request != null)
            {
                if (request.SensitiveData.HasValue)
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(
                        "SensitiveData",
                        application.SensitiveData,
                        new NewEntityActivity<ApplicationActivityType>(ApplicationActivityType.SensitivityChanged, request.SensitiveData)
                    ));
                }

                if (!string.IsNullOrEmpty(request.Status))
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(
                        "Status",
                        application.Status,
                        new NewEntityActivity<ApplicationActivityType>(ApplicationActivityType.StatusChanged, request.Status)
                    ));
                }

                if (!string.IsNullOrEmpty(request.AssignedTo))
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(
                        "AssignedTo",
                        application.AssignedTo,
                        new NewEntityActivity<ApplicationActivityType>(ApplicationActivityType.AssignedTo, request.AssignedTo)
                    ));
                }

                if (request.Assessment?.BedroomNeed.HasValue == true)
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(
                        "Assessment.BedroomNeed",
                        application.Assessment.BedroomNeed,
                        new NewEntityActivity<ApplicationActivityType>(ApplicationActivityType.BedroomNeedChanged, request.Assessment.BedroomNeed)
                    ));
                }

            }
            return activities;
        }
    }
}
