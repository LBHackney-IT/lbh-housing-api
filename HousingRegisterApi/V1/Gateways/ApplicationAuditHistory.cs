using Hackney.Core.Http;
using Hackney.Core.JWT;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace HousingRegisterApi.V1.Gateways
{
    public class ApplicationAuditHistory : IAuditHistory
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHttpContextWrapper _contextWrapper;
        private readonly ITokenFactory _tokenFactory;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public ApplicationAuditHistory(
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

        public void AuditUpdate(Guid id, UpdateApplicationRequest application)
        {
            //TODO: we want to only after submission, but for now, test if it works
            var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(_contextAccessor.HttpContext));

            _snsGateway.Publish(_snsFactory.Update(id, application, token));
        }
    }
}
