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
            IActivityHistory applicationHistory)
        {
            _gateway = gateway;
            _biddingNumberGenerator = biddingNumberGenerator;
            _applicationHistory = applicationHistory;
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
            var origApplication = _gateway.GetApplicationById(id);
            var activities = GetApplicationActivities(origApplication, request);

            var application = _gateway.UpdateApplication(id, request);
            if (null != application)
            {
                // audit the update
                _applicationHistory.LogActivity(application, activities);
            }

            return application.ToResponse();
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
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.SensitivityChangedByUser,
                        "SensitiveData", application.SensitiveData, request.SensitiveData));
                }

                if (!string.IsNullOrEmpty(request.Status))
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.StatusChangedByUser,
                        "Status", application.Status, request.Status));
                }

                if (!string.IsNullOrEmpty(request.AssignedTo))
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.AssignedToChangedByUser,
                        "AssignedTo", application.AssignedTo, request.AssignedTo));
                }

                if (request.Assessment?.BedroomNeed.HasValue == true)
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.BedroomNeedChangedByUser,
                        "Assessment.BedroomNeed", application.Assessment.BedroomNeed, request.Assessment.BedroomNeed));
                }

                if (request.Assessment?.EffectiveDate.HasValue == true)
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.EffectiveDateChangedByUser,
                        "Assessment.EffectiveDate", application.Assessment.EffectiveDate, request.Assessment.EffectiveDate));
                }
            }

            return activities;
        }
    }
}
