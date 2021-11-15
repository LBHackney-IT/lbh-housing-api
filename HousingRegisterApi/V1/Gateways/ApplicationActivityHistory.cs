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

        public void LogActivity(Guid applicationId, EntityActivity<ApplicationActivityType> activity)
        {
            if (activity != null)
            {
                this.LogActivity(applicationId, activity.OldData, activity.NewData);
            }
        }

        public void LogActivity(Guid applicationId, EntityActivityCollection<ApplicationActivityType> activities)
        {
            if (activities.Any())
            {
                this.LogActivity(applicationId, activities.OldData, activities.NewData);
            }
        }

        private void LogActivity(Guid applicationId, object oldData, object newData)
        {
            var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(_contextAccessor.HttpContext));

            var applicationSnsMessage = _snsFactory.Update(applicationId, oldData, newData, token);
            _snsGateway.Publish(applicationSnsMessage);
        }
    }
}
