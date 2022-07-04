using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Boundary.Response.Exceptions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.UseCase
{
    public class UpdateApplicationUseCase : IUpdateApplicationUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly IBiddingNumberGenerator _biddingNumberGenerator;
        private readonly IActivityGateway _activityGateway;

        public UpdateApplicationUseCase(
            IApplicationApiGateway gateway,
            IBiddingNumberGenerator biddingNumberGenerator,
            IActivityGateway applicationHistory)
        {
            _gateway = gateway;
            _biddingNumberGenerator = biddingNumberGenerator;
            _activityGateway = applicationHistory;
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
                activities.ForEach(activity =>
                {
                    _activityGateway.LogActivity(application, activity);
                });
            }

            return application.ToResponse();
        }

        /// <summary>
        /// Get a collection of all update activites performed on an application
        /// </summary>
        /// <param name="application"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private static List<EntityActivity<ApplicationActivityType>> GetApplicationActivities(Application application, UpdateApplicationRequest request)
        {
            var activities = new List<EntityActivity<ApplicationActivityType>>();

            if (application != null && request != null)
            {
                if (request.SensitiveData.HasValue)
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.SensitivityChangedByUser,
                        "SensitiveData", application.SensitiveData, request.SensitiveData));
                }

                if (!string.IsNullOrEmpty(request.Status))
                {
                    var activity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.StatusChangedByUser,
                        "Status", application.Status, request.Status);

                    activity.AddChange("Assessment.Reason", application.Assessment?.Reason, request.Assessment?.Reason);

                    activities.Add(activity);
                }

                if (!string.IsNullOrEmpty(request.AssignedTo))
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.AssignedToChangedByUser,
                        "AssignedTo", application.AssignedTo, request.AssignedTo));
                }

                if (request.Assessment?.BedroomNeed.HasValue == true)
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.BedroomNeedChangedByUser,
                        "Assessment.BedroomNeed", application.Assessment?.BedroomNeed, request.Assessment.BedroomNeed));
                }

                if (request.Assessment?.EffectiveDate.HasValue == true)
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.EffectiveDateChangedByUser,
                        "Assessment.EffectiveDate", application.Assessment?.EffectiveDate, request.Assessment.EffectiveDate));
                }

                if (!string.IsNullOrEmpty(request.Assessment?.Band))
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.BandChangedByUser,
                        "Assessment.Band", application.Assessment?.Band, request.Assessment.Band));
                }

                if (request.Assessment?.InformationReceivedDate.HasValue == true)
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.InformationReceivedDateChangedByUser,
                        "Assessment.InformationReceivedDate", application.Assessment?.InformationReceivedDate, request.Assessment.InformationReceivedDate));
                }

                if (!string.IsNullOrEmpty(request.Assessment?.BiddingNumber))
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.BiddingNumberChangedByUser,
                        "Assessment.BiddingNumber", application.Assessment?.BiddingNumber, request.Assessment.BiddingNumber));
                }

                /*if (request.MainApplicant != null)
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.MainApplicantChangedByUser,
                        "MainApplicant", application.MainApplicant, request.MainApplicant));
                }*/

                if (request.MainApplicant?.Person?.FirstName != null)
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.PersonChangedByUser,
                        "MainApplicant.Person.Firstname", application.MainApplicant?.Person?.FirstName, request.MainApplicant?.Person?.FirstName));
                }

                //Remove all of the above and check the application against the request.
                //Change EntityActivity to accept a sring instead of an ApplicationActivityType
                /*Applicant oldApplicant = application.MainApplicant;
                Applicant newApplicant = request.MainApplicant;

                var changes = ObjectExtensions.GetChangedProperties<Applicant>(oldApplicant, newApplicant);
                if (changes.Count > 0)
                {
                    List<string> changeList = new List<string>();
                    foreach (var change in changes)
                    {
                        changeList.Add(change + ": " + "");
                    }

                }*/
            }

            return activities;
        }
    }
}
