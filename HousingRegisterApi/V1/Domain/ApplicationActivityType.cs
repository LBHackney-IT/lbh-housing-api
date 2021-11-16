using System.ComponentModel;

namespace HousingRegisterApi.V1.Domain
{
    public enum ApplicationActivityType
    {
        [Description("Submitted")]
        Submitted = 0,

        [Description("CaseViewed")]
        CaseViewed = 1,

        [Description("StatusChanged")]
        StatusChanged = 2,

        [Description("AssignedTo")]
        AssignedTo = 3,

        [Description("CaseActivated")]
        CaseActivated = 4,

        [Description("CaseRejected")]
        CaseRejected = 5,

        [Description("SensitivityChanged")]
        SensitivityChanged = 6,

        [Description("BedroomNeedChanged")]
        BedroomNeedChanged = 7,

        [Description("DateChanged")]
        DateChanged = 8,
    }
}
