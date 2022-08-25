using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Boundary.Response.Exceptions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Gateways.Interfaces;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class UpdateApplicationUseCase : IUpdateApplicationUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly IActivityGateway _activityGateway;
        private readonly ISearchGateway _search;

        public UpdateApplicationUseCase(
            IApplicationApiGateway gateway,
            IActivityGateway applicationHistory,
            ISearchGateway search)
        {
            _gateway = gateway;
            _activityGateway = applicationHistory;
            _search = search;
        }

        public async Task<ApplicationResponse> Execute(Guid id, UpdateApplicationRequest request)
        {
            //Validate dates are whole dates
            ValidateDates(request);

            var origApplication = _gateway.GetApplicationById(id);

            bool biddingNumberChanged = origApplication?.Assessment?.BiddingNumber != request?.Assessment?.BiddingNumber;

            //Check the bidding number if it has changed, or 
            if (request.Assessment != null
                && request.Assessment.GenerateBiddingNumber || biddingNumberChanged)
            {

                if (request.Assessment.GenerateBiddingNumber && string.IsNullOrWhiteSpace(request.Assessment.BiddingNumber))
                {
                    //There is no specified bidding number in the update, and auto-generate is set to true
                    var newBiddingNumber = await _gateway.IssueNextBiddingNumber().ConfigureAwait(false);

                    request.Assessment.BiddingNumber = newBiddingNumber.ToString();
                }
                else if (biddingNumberChanged)
                {
                    //The bidding number has been manually changed.  This newly entered bidding number needs to be validated.
                    long newBiddingNumber = 0;
                    if (!long.TryParse(request?.Assessment?.BiddingNumber, out newBiddingNumber))
                    {
                        //The user has supplied a bidding number which is not a number
                        throw new InvalidBiddingNumberException($"Supplied bidding number \"{request.Assessment.BiddingNumber}\" is invalid");
                    }
                    else if (newBiddingNumber > 0)
                    {
                        //The user has manually supplied a bidding number -check its not in the auto generated range, and check for duplicates

                        //First, check that its not in the auto-generate range
                        var lastIssuedBiddingNumber = await _gateway.GetLastIssuedBiddingNumber().ConfigureAwait(false);

                        if (lastIssuedBiddingNumber.HasValue)
                        {
                            if (newBiddingNumber > lastIssuedBiddingNumber.Value)
                            {
                                throw new InvalidBiddingNumberException($"Supplied bidding number \"{request.Assessment.BiddingNumber}\" is reserved for auto-generation.  Please use a bidding number below {lastIssuedBiddingNumber.Value}, or auto-generate the bidding number");
                            }
                        }

                        //Then, check for duplicates
                        var duplicateCases = await _search.GetByBiddingNumber(newBiddingNumber, 0, 10).ConfigureAwait(false);

                        if (duplicateCases.TotalResults > 0)
                        {
                            string references = string.Join(",", duplicateCases.Results.Select(r => r.Reference));
                            throw new DuplicateBiddingNumberException($"The bidding number {newBiddingNumber} is a duplicate - it is used on {duplicateCases.TotalResults} other case(s). References: {references}");
                        }
                    }
                }
            }

            // get list of all update activities prior to updating the application

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

        private static void ValidateDates(UpdateApplicationRequest request)
        {
            if ((request?.MainApplicant?.Person?.DateOfBirth.Hour ?? 0) != 0)
            {
                //Date of birth has an hour component - this time has been skewed by timezone issues on front end - reject
                throw new InvalidDateException($"The main applicant D.O.B has an invalid timezone skew - {request.MainApplicant.Person.DateOfBirth}");
            }

            foreach (var otherApplicant in request.OtherMembers)
            {
                if ((otherApplicant?.Person.DateOfBirth.Hour ?? 0) != 0)
                {
                    //Date of birth has an hour component - this time has been skewed by timezone issues on front end - reject
                    throw new InvalidDateException($"The houshold member {otherApplicant.Person?.FullName}'s D.O.B has an invalid timezone skew - {otherApplicant.Person.DateOfBirth}");
                }

                if (otherApplicant?.MedicalNeed?.AssessmentDate.HasValue ?? false)
                {
                    if (otherApplicant.MedicalNeed.AssessmentDate.Value.Hour != 0)
                    {
                        //Date of birth has an hour component - this time has been skewed by timezone issues on front end - reject
                        throw new InvalidDateException($"The houshold member {otherApplicant.Person?.FullName}'s Medical Need assessment date has an invalid timezone skew - {otherApplicant.MedicalNeed.AssessmentDate.Value}");
                    }
                }

                if ((otherApplicant?.MedicalNeed?.FormRecieved.Hour ?? 0) != 0)
                {
                    //Date of birth has an hour component - this time has been skewed by timezone issues on front end - reject
                    throw new InvalidDateException($"The houshold member {otherApplicant.Person?.FullName}'s Medical Need Form Received date has an invalid timezone skew - {otherApplicant.MedicalNeed.FormRecieved}");
                }
            }

            if (request?.Assessment?.InformationReceivedDate.HasValue ?? false)
            {
                if (request.Assessment.InformationReceivedDate.Value.Hour != 0)
                {
                    //Date of birth has an hour component - this time has been skewed by timezone issues on front end - reject
                    throw new InvalidDateException($"The assessments information recieved date has an invalid timezone skew - {request.Assessment.InformationReceivedDate.Value}");
                }
            }

            if (request?.Assessment?.EffectiveDate.HasValue ?? false)
            {
                if (request.Assessment.EffectiveDate.Value.Hour != 0)
                {
                    //Date of birth has an hour component - this time has been skewed by timezone issues on front end - reject
                    throw new InvalidDateException($"The assessments Effective date has an invalid timezone skew - {request.Assessment.EffectiveDate.Value}");
                }
            }

            if (request.MainApplicant?.MedicalNeed != null)
            {
                if (request.MainApplicant.MedicalNeed?.AssessmentDate.HasValue ?? false)
                {
                    if (request.MainApplicant.MedicalNeed.AssessmentDate.Value.Hour != 0)
                    {
                        //Date of birth has an hour component - this time has been skewed by timezone issues on front end - reject
                        throw new InvalidDateException($"The main applicants Medical Need assessment date has an invalid timezone skew - {request.MainApplicant.MedicalNeed.AssessmentDate.Value}");
                    }
                }

                if ((request.MainApplicant?.MedicalNeed?.FormRecieved.Hour ?? 0) != 0)
                {
                    //Date of birth has an hour component - this time has been skewed by timezone issues on front end - reject
                    throw new InvalidDateException($"The main applicants Medical Need Form Received date has an invalid timezone skew - {request.MainApplicant.MedicalNeed.FormRecieved}");
                }
            }
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

                if (request.MainApplicant != null)
                {
                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.MainApplicantChangedByUser,
                        "MainApplicant", application.MainApplicant, request.MainApplicant));
                }

                if (request.OtherMembers != null)
                {
                    if (application.OtherMembers != null)
                    {
                        //New member added?
                        foreach (var requestMember in request.OtherMembers)
                        {
                            var applicationMember = application.OtherMembers.SingleOrDefault(om => om.Person.Id == requestMember.Person.Id);
                            if (applicationMember == null)
                            {
                                //member is newly added
                                activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.HouseholdApplicantChangedByUser,
                        $"OtherMembers[{requestMember.Person.Id}]", null, requestMember));
                            }
                            else
                            {
                                //Member already exists - check if any properties have changed
                                var differences = Compare(requestMember, applicationMember);
                                if (differences.Any())
                                {
                                    //This person has changed, add activity type
                                    activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.HouseholdApplicantChangedByUser,
                        $"OtherMembers[{requestMember.Person.Id}]", applicationMember, requestMember));
                                }
                            }
                        }
                    }
                    else
                    {
                        //First time other members added
                        foreach (var member in request.OtherMembers)
                        {
                            activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.HouseholdApplicantChangedByUser,
                    $"OtherMembers[{member.Person.Id}]", null, member));
                        }
                    }

                    //Check for household members that have been removed
                    foreach (var applicationMember in application.OtherMembers)
                    {
                        var requestMember = request.OtherMembers.SingleOrDefault(om => om.Person.Id == applicationMember.Person.Id);
                        if (requestMember == null)
                        {
                            activities.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.HouseholdApplicantRemovedByUser,
                    $"OtherMembers[{applicationMember.Person.Id}]", applicationMember, null));
                        }
                    }
                }
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


            return activities;
        }

        private static List<Variance> Compare<T>(T objectA, T objectB)
        {
            var variances = new List<Variance>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var v = new Variance
                {
                    PropertyName = property.Name,
                    ValueA = property.GetValue(objectA),
                    ValueB = property.GetValue(objectB)
                };
                if (v.ValueA == null && v.ValueB == null)
                {
                    continue;
                }
                if (
                    (v.ValueA == null && v.ValueB != null)
                    ||
                    (v.ValueA != null && v.ValueB == null)
                )
                {
                    variances.Add(v);
                    continue;
                }
                if (!v.ValueA.Equals(v.ValueB))
                {
                    variances.Add(v);
                }
            }
            return variances;
        }

    }
    internal class Variance
    {
        public string PropertyName { get; set; }
        public object ValueA { get; set; }
        public object ValueB { get; set; }
    }
}
