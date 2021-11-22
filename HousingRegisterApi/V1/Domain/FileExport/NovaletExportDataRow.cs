using System;

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
            RegistrationDate = FormatDate(application.SubmittedAt);
            EffectiveDate = FormatDate(application.Assessment?.EffectiveDate);
            ApplicantType = application.Assessment?.Band;
            MinimumBedSize = bedroomNeed.HasValue ? Math.Max(0, bedroomNeed.Value - 1).ToString() : null;
            MaximumBedSize = bedroomNeed.HasValue ? Math.Max(0, bedroomNeed.Value).ToString() : null;
            DateOfBirth = FormatDate(application.MainApplicant?.Person?.DateOfBirth);
            OlderPersonsAssessement = null;
            MobilityAssessment = null;
            AdditionalBandingInfo = null;
            MedicalRequirements = null;
            Offered = null;
            EthnicOrigin = GetEthnicity(application);
            Decant = null;
            AHRCode = null;
            AutoBidPref_MobilityStandard = null;
            AutoBidPref_WheelChairStandard = null;
            AutoBidPref_AdaptedStandard = null;
        }

        private static string FormatDate(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                return dateTime.Value.ToString("ddMMyyyy");
            }

            return null;
        }

        private static string GetEthnicity(Application application)
        {
            var ethnicityCategoryKey = "ethnicity-questions/ethnicity-main-category";
            var extended = "ethnicity-extended-category";

            var ethnicityKey = (application.MainApplicant.Questions.GetAnswer(ethnicityCategoryKey)) switch
            {
                PersonEthnicityCategory.AsianOrAsianBritish => $"ethnicity-extended-category-asian-asian-british/{extended}",
                PersonEthnicityCategory.BlackOrBlackBritish => $"ethnicity-extended-category-black-black-british/{extended}",
                PersonEthnicityCategory.MixedOrMultipleBackground => $"ethnicity-extended-category-mixed-multiple-background/{extended}",
                PersonEthnicityCategory.OtherEthnicGroup => $"ethnicity-extended-category-white/{extended}",
                PersonEthnicityCategory.White => $"ethnicity-extended-category-other-ethnic-group/{extended}",
                PersonEthnicityCategory.PreferNotToSay => null,
                _ => null,
            };

            if (ethnicityKey == null)
            {
                return "PRE";
            }

            var ethnicity = application.MainApplicant.Questions.GetAnswer(ethnicityKey);

            return ethnicity switch
            {
                PersonEthnicity.AsianIndian => "IND",
                PersonEthnicity.AsianPakistani => "PAK",
                PersonEthnicity.AsianBangladeshi => "BAN",
                PersonEthnicity.AsianChinese => "CHI",
                PersonEthnicity.AsianVietnamese => "VIE",
                PersonEthnicity.AsianOther => "AOA",
                PersonEthnicity.AsianNepali => "AOA",
                PersonEthnicity.AsianSriLankanOther => "AOA",
                PersonEthnicity.AsianSriLankanTamil => "AOA",
                PersonEthnicity.AsianSriLankanSinhalese => "AOA",

                PersonEthnicity.BlackCaribbean => "CAR",
                PersonEthnicity.BlackBritish => "EWS",
                PersonEthnicity.BlackCongolese => "CON",
                PersonEthnicity.BlackGhanaian => "GHA",
                PersonEthnicity.BlackNigerian => "KUR",
                PersonEthnicity.BlackSomali => "SOM",
                PersonEthnicity.BlackAngolan => "AOB",
                PersonEthnicity.BlackSudanese => "AOB",
                PersonEthnicity.BlackAfricanOther => "AOB",
                PersonEthnicity.BlackSierraLeonean => "AOB",

                PersonEthnicity.WhiteAndBlackCaribbean => "WBC",
                PersonEthnicity.WhiteAndBlackAfrican => "WBA",
                PersonEthnicity.WhiteAndAsian => "WAS",
                PersonEthnicity.OtherMixedBackground => "AOM",

                PersonEthnicity.WhiteBritish => "EWS",
                PersonEthnicity.WhiteCharediJew => "JEW",
                PersonEthnicity.WhiteOrthodoxJew => "OJE",
                PersonEthnicity.WhiteGreekCypriot => "GCY",
                PersonEthnicity.WhiteIrish => "IRI",
                PersonEthnicity.WhiteTurkishCypriot => "TCY",
                PersonEthnicity.WhiteOtherEasternEuropean => "EEU",
                PersonEthnicity.WhiteOtherWesternEuropean => "OEU",
                PersonEthnicity.WhiteGypsyOrIrishTraveller => "IRT",
                PersonEthnicity.WhiteOther => "AOW",
                PersonEthnicity.WhitePolish => "AOW",
                PersonEthnicity.WhiteTurkish => "AOW",
                PersonEthnicity.WhiteItalian => "AOW",
                PersonEthnicity.WhiteNorthAmerican => "AOW",
                PersonEthnicity.WhiteAustralianNewZealander => "AOW",

                PersonEthnicity.OtherKurdish => "KUR",
                PersonEthnicity.OtherEuropean => "OEU",
                PersonEthnicity.OtherGypsyRoma => "GRO",
                PersonEthnicity.OtherVietnamese => "VIE",
                PersonEthnicity.OtherThai => "ARA",
                PersonEthnicity.OtherArab => "ARA",
                PersonEthnicity.OtherMalay => "ARA",
                PersonEthnicity.OtherIraqi => "ARA",
                PersonEthnicity.OtherKorean => "ARA",
                PersonEthnicity.OtherAfghan => "ARA",
                PersonEthnicity.OtherLibyan => "ARA",
                PersonEthnicity.OtherYemeni => "ARA",
                PersonEthnicity.OtherIranian => "ARA",
                PersonEthnicity.OtherTurkish => "ARA",
                PersonEthnicity.OtherEgyptian => "ARA",
                PersonEthnicity.OtherFilipino => "ARA",
                PersonEthnicity.OtherJapanese => "ARA",
                PersonEthnicity.OtherLebanese => "ARA",
                PersonEthnicity.OtherMoroccan => "ARA",
                PersonEthnicity.OtherPolynesian => "ARA",
                PersonEthnicity.OtherLatinSouthCentralAmerican => "ARA",
                _ => "AOO",
            };
        }
    }
}
