using System.ComponentModel;

namespace HousingRegisterApi.V1.Domain
{
    public enum ApplicationStatus
    {
        Verification,
        New,
        Submitted,
        Active,
        Pending,
        Referred,
        Rejected,
        Disqualified,
        Cancelled,
        Suspended,
        Housed,
        ActiveUnderAppeal,
        InactiveUnderAppeal,
    }
}
