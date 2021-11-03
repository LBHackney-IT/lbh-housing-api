using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.Sns;
using HousingRegisterApi.V1.Infrastructure;
using System;

namespace HousingRegisterApi.V1.Factories
{
    public class ApplicationSnsFactory : ISnsFactory
    {
        public ApplicationSns Create(Application application, string token)
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
                    NewData = application
                }
            };
        }
    }
}
