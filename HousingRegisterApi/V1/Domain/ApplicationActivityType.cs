using System.ComponentModel;

namespace HousingRegisterApi.V1.Domain
{
    public enum ApplicationActivityType
    {
        [Description("SubmittedByResident")]
        SubmittedByResident = 0,

        [Description("CaseViewedByUser")]
        CaseViewedByUser = 1,

        [Description("StatusChangedByUser")]
        StatusChangedByUser = 2,

        [Description("UpdateAssignedToByUser")]
        AssignedToChangedByUser = 3,

        [Description("SensitivityChangedByUser")]
        SensitivityChangedByUser = 4,

        [Description("BedroomNeedChangedByUser")]
        BedroomNeedChangedByUser = 5,

        [Description("EffectiveDateChangedByUser")]
        EffectiveDateChangedByUser = 6,

        [Description("NoteAddedByUser")]
        NoteAddedByUser = 7,

        [Description("ImportedFromLegacyDatabase")]
        ImportedFromLegacyDatabase = 8,

        [Description("BandChangedByUser")]
        BandChangedByUser = 9,

        [Description("InformationReceivedDateChangedByUser")]
        InformationReceivedDateChangedByUser = 10,

        [Description("BiddingNumberChangedByUser")]
        BiddingNumberChangedByUser = 11,

        [Description("MainApplicantChangedByUser")]
        MainApplicantChangedByUser = 12,

        [Description("HouseholdApplicantChangedByUser")]
        HouseholdApplicantChangedByUser = 13,

        [Description("HouseholdApplicantRemovedByUser")]
        HouseholdApplicantRemovedByUser = 14,

        [Description("Created")]
        Created = 15
    }
}
