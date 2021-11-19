using System;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Domain.FileExport
{
    public class NovaletExportDataRow
    {
        public string HousingRegisterRef { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address5 { get; set; }
        public string Postcode { get; set; }
        public string HomeTelephone { get; set; }
        public string WorkTelephone { get; set; }
        public string Email { get; set; }
        public string NINumber { get; set; }
        public string Sex { get; set; }
        public string RegistrationDate { get; set; }
        public string EffectiveDate { get; set; }
        public string ApplicantType { get; set; }
        public string MinimumBedSize { get; set; }
        public string MaximumBedSize { get; set; }
        public string DateOfBirth { get; set; }
        public string OlderPersonsAssessement { get; set; }
        public string MobilityAssessment { get; set; }
        public string AdditionalBandingInfo { get; set; }
        public string MedicalRequirements { get; set; }
        public string Offered { get; set; }
        public string EthnicOrigin { get; set; }
        public string Decant { get; set; }
        public string AHRCode { get; set; }
        public string AutoBidPref_MobilityStandard { get; set; }
        public string AutoBidPref_WheelChairStandard { get; set; }
        public string AutoBidPref_AdaptedStandard { get; set; }

        public NovaletExportDataRow(Application application)
        {
            var bedroomNeed = application.Assessment?.BedroomNeed ?? application.CalculatedBedroomNeed!;

            HousingRegisterRef = application.Assessment?.BiddingNumber ?? null;
            Title = application.MainApplicant?.Person?.Title ?? null;
            FirstName = application.MainApplicant?.Person?.FirstName ?? null;
            FamilyName = application.MainApplicant?.Person?.Surname ?? null;
            Address1 = application.MainApplicant?.Address?.AddressLine1 ?? null;
            Address2 = application.MainApplicant?.Address?.AddressLine2 ?? null;
            Address3 = application.MainApplicant?.Address?.AddressLine3 ?? null;
            Address4 = null;
            Address5 = null;
            Postcode = application.MainApplicant?.Address?.Postcode ?? null;
            HomeTelephone = application.MainApplicant?.ContactInformation?.PhoneNumber ?? null;
            WorkTelephone = null;
            Email = application.MainApplicant?.ContactInformation?.EmailAddress ?? null;
            NINumber = application.MainApplicant?.Person?.NationalInsuranceNumber ?? null;
            Sex = application.MainApplicant?.Person?.Gender ?? null;
            RegistrationDate = application.SubmittedAt?.ToString();
            EffectiveDate = application.Assessment?.EffectiveDate.ToString();
            ApplicantType = GetApplicantType(application);
            MinimumBedSize = bedroomNeed.HasValue ? Math.Max(0, bedroomNeed.Value - 1).ToString() : null;
            MaximumBedSize = bedroomNeed.HasValue ? Math.Max(0, bedroomNeed.Value).ToString() : null;
            DateOfBirth = application.MainApplicant?.Person?.DateOfBirth.ToString();
            OlderPersonsAssessement = null;
            MobilityAssessment = null;
            AdditionalBandingInfo = null;
            MedicalRequirements = null;
            Offered = null;
            EthnicOrigin = null;
            Decant = null;
            AHRCode = null;
            AutoBidPref_MobilityStandard = null;
            AutoBidPref_WheelChairStandard = null;
            AutoBidPref_AdaptedStandard = null;
        }

        private static string GetApplicantType(Application application)
        {
            switch (application.Assessment?.Band)
            {
                case AssessmentBand.BandA: return "EMG";
                case AssessmentBand.BandB: return "SHN";
                case AssessmentBand.BandC: return "SCH";
                default: return null;
            }
        }
    }
}
