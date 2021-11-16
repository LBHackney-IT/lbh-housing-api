using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Infrastructure;
using System;

namespace HousingRegisterApi.V1.Gateways
{
    public interface IActivityHistory
    {
        void LogUpdate(Guid applicationId, EntityActivityCollection<ApplicationActivityType> activities);
    }
}
