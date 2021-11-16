using System.ComponentModel;

namespace HousingRegisterApi.V1.Domain
{
    public enum ApplicationActivityType
    {
        [Description("Submitted")]
        Submitted,

        [Description("CaseViewed")]
        CaseViewed,

        [Description("StatusChanged")]
        StatusChanged,

        [Description("AssignedTo")]
        AssignedTo,

        [Description("CaseActivated")]
        CaseActivated,

        [Description("CaseRejected")]
        CaseRejected,

        [Description("SensitivityChanged")]
        SensitivityChanged,

        [Description("BedroomNeedChanged")]
        BedroomNeedChanged,

        [Description("DateChanged")]
        DateChanged,
    }
}
