using Hackney.Core.JWT;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.Sns;
using HousingRegisterApi.V1.Infrastructure;
using System;

namespace HousingRegisterApi.V1.Factories
{
    public class ApplicationSnsFactory : ISnsFactory
    {
        public ApplicationSns Create(Application application, Token token)
        {
            return new ApplicationSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = application.Id,
                Id = Guid.NewGuid(),
                EventType = CreateApplicationConstants.EVENTTYPE,
                Version = CreateApplicationConstants.V1VERSION,
                SourceDomain = CreateApplicationConstants.SOURCEDOMAIN,
                SourceSystem = CreateApplicationConstants.SOURCESYSTEM,
                User = new UserSns
                {
                    Name = token.Name,
                    Email = token.Email
                },
                EventData = new EventData
                {
                    NewData = application,
                }
            };
        }

        public ApplicationSns Update(Guid id, UpdateApplicationRequest application, Token token)
        {
            return new ApplicationSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = id,
                Id = Guid.NewGuid(),
                EventType = UpdateApplicationConstants.EVENTTYPE,
                Version = UpdateApplicationConstants.V1VERSION,
                SourceDomain = UpdateApplicationConstants.SOURCEDOMAIN,
                SourceSystem = UpdateApplicationConstants.SOURCESYSTEM,
                User = new UserSns
                {
                    Name = "",
                    Email = ""
                },
                EventData = new EventData
                {
                    NewData = application,
                    //NewData = updateResult.NewValues
                }
            };
        }
    }
}
