using HousingRegisterApi.V1.Boundary.Request;
using System;

namespace HousingRegisterApi.V1.Gateways.Interfaces
{
    public interface IAuditHistory
    {
        void AuditUpdate(Guid id, UpdateApplicationRequest application);
    }
}
