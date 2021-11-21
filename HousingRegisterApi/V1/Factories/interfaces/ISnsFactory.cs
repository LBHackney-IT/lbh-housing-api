using Hackney.Core.JWT;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.Sns;
using System;

namespace HousingRegisterApi.V1.Factories
{
    public interface ISnsFactory
    {
        ApplicationSns Create(Application application, Token token);

        ApplicationSns Update(Guid id, object oldValue, object newValues, Token token);
    }
}
