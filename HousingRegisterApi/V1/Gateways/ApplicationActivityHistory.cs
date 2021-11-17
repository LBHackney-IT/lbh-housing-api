using Hackney.Core.Http;
using Hackney.Core.JWT;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;

namespace HousingRegisterApi.V1.Gateways
{
    public class ApplicationActivityHistory : IActivityHistory
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHttpContextWrapper _contextWrapper;
        private readonly ITokenFactory _tokenFactory;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public ApplicationActivityHistory(
            IHttpContextAccessor contextAccessor,
            IHttpContextWrapper contextWrapper,
            ITokenFactory tokenFactory,
            ISnsGateway snsGateway,
            ISnsFactory snsFactory)
        {
            _contextAccessor = contextAccessor;
            _contextWrapper = contextWrapper;
            _tokenFactory = tokenFactory;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public void LogActivity(Application application, EntityActivity<ApplicationActivityType> activity)
        {
            if (activity != null)
            {               
                bool isResidentActivity = activity.ActivityType == ApplicationActivityType.SubmittedByResident;
                this.LogActivity(application, activity.OldData, activity.NewData, isResidentActivity);
            }
        }

        public void LogActivity(Application application, EntityActivityCollection<ApplicationActivityType> activities)
        {
            if (activities.Any())
            {
                this.LogActivity(application, activities.OldData, activities.NewData);
            }
        }

        /// <summary>
        /// LogActivity
        /// </summary>
        /// <param name="application"></param>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        /// <param name="isResidentActivity"></param>
        private void LogActivity(Application application, object oldData, object newData, bool isResidentActivity = false)
        {
            // we only want to log activites after an application has been submitted
            if (application != null &&
                (application.Status != ApplicationStatus.Verification
                || application.Status != ApplicationStatus.New))
            {
                var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(_contextAccessor.HttpContext));

                // residents will not have an auth token so
                // generate a simple token to hold some user info
                if (isResidentActivity == true && token == null)
                {                   
                    token = new Token()
                    {
                        Name = application.MainApplicant.Person.FullName,
                        Email = application.MainApplicant.ContactInformation.EmailAddress,
                    };
                }

                if (token != null)
                {
                    var applicationSnsMessage = _snsFactory.Update(application.Id, oldData, newData, token);
                    _snsGateway.Publish(applicationSnsMessage);
                }
            }
        }
    }
}
