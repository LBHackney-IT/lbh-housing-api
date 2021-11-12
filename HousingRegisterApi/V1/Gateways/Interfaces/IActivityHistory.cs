using HousingRegisterApi.V1.Boundary.Request;
using System;

namespace HousingRegisterApi.V1.Gateways.Interfaces
{
    public interface IActivityHistory
    {
        void LogUpdate(Guid id, UpdateApplicationRequest application);
    }
}
