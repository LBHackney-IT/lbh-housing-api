using Hackney.Core.Http;
using Hackney.Core.JWT;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

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

        public void LogUpdate(Guid applicationId, EntityActivityCollection<ApplicationActivityType> activities)
        {
            if (activities.Any())
            {
                //TODO: we want to only after submission, but for now, test if it works
                var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(_contextAccessor.HttpContext));

                var applicationSnsMessage = _snsFactory.Update(applicationId, activities.OldData(), activities.NewData(), token);
                _snsGateway.Publish(applicationSnsMessage);
            }
        }
    }
}
