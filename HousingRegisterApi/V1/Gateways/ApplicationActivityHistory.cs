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
                this.LogActivity(application, activity.OldData, activity.NewData);
            }
        }

        public void LogActivity(Application application, EntityActivityCollection<ApplicationActivityType> activities)
        {
            if (activities.Any())
            {
                this.LogActivity(application, activities.OldData, activities.NewData);
            }
        }

        private void LogActivity(Application application, object oldData, object newData)
        {
            // we only want to log activites after an application has been submitted
            if (application != null &&
                (application.Status != ApplicationStatus.Verification
                || application.Status != ApplicationStatus.New))
            {
                var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(_contextAccessor.HttpContext));

                if (token != null)
                {
                    var applicationSnsMessage = _snsFactory.Update(application.Id, oldData, newData, token);
                    _snsGateway.Publish(applicationSnsMessage);
                }
            }
        }
    }
}
