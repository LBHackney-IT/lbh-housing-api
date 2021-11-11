using HousingRegisterApi.V1.Domain;

namespace HousingRegisterApi.V1.Gateways.Interfaces
{
    public interface IAuditHistory
    {
        void AuditUpdate(Application application);
    }
}
