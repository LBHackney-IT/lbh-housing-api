using HousingRegisterApi.V1.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HousingRegisterApi.V1.Domain.Report
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
        public static string ErrorData { get; set; }

        public string Errors { get; set; }

        public NovaletExportDataRow(Application application)
        {
            var bedroomNeed = application.Assessment?.BedroomNeed ?? application.CalculatedBedroomNeed!;

            HousingRegisterRef = application.Assessment?.BiddingNumber?.ToString();
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
            if (application.MainApplicant?.Person?.NationalInsuranceNumber != null)
            {
                NINumber = Regex.Replace(application.MainApplicant?.Person?.NationalInsuranceNumber, @"\s", "").ToUpper();
            }
            else
            {
                NINumber = null;
            }
            Sex = application.MainApplicant?.Person?.Gender ?? null;
            RegistrationDate = FormatDate(application.SubmittedAt);
            EffectiveDate = FormatDate(application.Assessment?.EffectiveDate);
            ApplicantType = GetBand(application.Assessment?.Band);
            MinimumBedSize = bedroomNeed.HasValue ? Math.Max(0, bedroomNeed.Value - 1).ToString() : null;
            MaximumBedSize = bedroomNeed.HasValue ? Math.Max(0, bedroomNeed.Value).ToString() : null;
            DateOfBirth = FormatDate(application.MainApplicant?.Person?.DateOfBirth);
            OlderPersonsAssessement = null;
            MobilityAssessment = null;
            AdditionalBandingInfo = null;
            MedicalRequirements = null;
            Offered = "N";
            EthnicOrigin = GetEthnicity(application);
            Decant = null;
            List<Applicant> applicants = new List<Applicant>();
            applicants.Add(application.MainApplicant);
            applicants.AddRange(application.OtherMembers);
            AHRCode = GetAHRCode(applicants.Where(a => a.MedicalNeed != null && a.MedicalNeed.AccessibileHousingRegister != null).ToList());
            AutoBidPref_MobilityStandard = AHRCode == "B" ? "Y" : "N";
            AutoBidPref_WheelChairStandard = AHRCode == "A" ? "Y" : "N";
            AutoBidPref_AdaptedStandard = AHRCode == "E" || AHRCode == "C" || AHRCode == "E+" || AHRCode == "G" || AHRCode == "D" ? "Y" : "N";
            Errors = ErrorData;
        }

        /**
AOB - A or B Wheelchair standard
COD - C or D level access with wide doors
EEE - E level access
EPL - E + Ground floor up to 4 steps
FLS - F any floor but with level access shower
EPS - E + ground floor and level access shower
STL - stairlift        
FFF - F any floor


        **/

        private static string FormatDate(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                return dateTime.Value.ToString("ddMMyyyy");
            }

            return null;
        }

        private static string GetAHRCode(List<Applicant> applicants)
        {
            string AHRCodePriority = "";

            if (applicants.Select(x => x.MedicalNeed.AccessibileHousingRegister).Contains("a-b-wheelchair-standard"))
            {
                AHRCodePriority = "A"; //AOB
            }
            else if (applicants.Select(x => x.MedicalNeed.AccessibileHousingRegister).Contains("c-d-large-level-access"))
            {
                AHRCodePriority = "B";//COD
            }
            else if (applicants.Select(x => x.MedicalNeed.AccessibileHousingRegister).Contains("e-step-free-level-access"))
            {
                AHRCodePriority = "E";//EEE
            }
            else if (applicants.Select(x => x.MedicalNeed.AccessibileHousingRegister).Contains("e-minimal-steps-up-to-4-steps"))
            {
                AHRCodePriority = "C";//EPL
            }
            else if (applicants.Select(x => x.MedicalNeed.AccessibileHousingRegister).Contains("e-ground-floor-and-level-access-shower"))
            {
                AHRCodePriority = "E+";//EPS
            }
            else if (applicants.Select(x => x.MedicalNeed.AccessibileHousingRegister).Contains("e-up-to-4-steps-and-level-access-shower"))
            {
                AHRCodePriority = "C";//EPL
            }
            else if (applicants.Select(x => x.MedicalNeed.AccessibileHousingRegister).Contains("stairlift"))
            {
                AHRCodePriority = "D";//STL
            }
            else if (applicants.Select(x => x.MedicalNeed.AccessibileHousingRegister).Contains("f-general-needs-any-floor"))
            {
                AHRCodePriority = "F";//FFF
            }
            else if (applicants.Select(x => x.MedicalNeed.AccessibileHousingRegister).Contains("f-any-floor-with-level-access-shower"))
            {
                AHRCodePriority = "G";//FLS
            }

            return AHRCodePriority;
        }

        private static string GetBand(string band)
        {
            return band switch
            {
                "A" => "Band A",
                "B" => "Band B",
                "C" => "Band C",
                "C-transitional" => "Band C",
                _ => null
            };
        }

        private static string GetEthnicity(Application application)
        {


            var legacyOverride = application.MainApplicant.Questions.GetAnswer("legacy-database/ethnicOrigin");
            if (!string.IsNullOrEmpty(legacyOverride))
            {
                return legacyOverride;
            }

            var ethnicityCategoryKey = "ethnicity-questions/ethnicity-main-category";
            var extended = "ethnicity-extended-category";

            var ethnicityCategoryKeyAnswer = application.MainApplicant.Questions.GetAnswer(ethnicityCategoryKey);
            if (ethnicityCategoryKeyAnswer.StartsWith("ERROR in answer:"))
            {
                ErrorData += (ethnicityCategoryKeyAnswer + " for application " + application.Id + " and reference " + application.Reference) + "\n";
            }

            var ethnicityKey = ethnicityCategoryKeyAnswer switch
            {
                PersonEthnicityCategory.AsianOrAsianBritish => $"ethnicity-extended-category-asian-asian-british/{extended}",
                PersonEthnicityCategory.BlackOrBlackBritish => $"ethnicity-extended-category-black-black-british/{extended}",
                PersonEthnicityCategory.MixedOrMultipleBackground => $"ethnicity-extended-category-mixed-multiple-background/{extended}",
                PersonEthnicityCategory.OtherEthnicGroup => $"ethnicity-extended-category-other-ethnic-group/{extended}",
                PersonEthnicityCategory.White => $"ethnicity-extended-category-white/{extended}",
                PersonEthnicityCategory.PreferNotToSay => null,
                _ => null,
            };

            if (ethnicityKey == null)
            {
                return "PRE";
            }

            var ethnicity = application.MainApplicant.Questions.GetAnswer(ethnicityKey);
            if (ethnicity.StartsWith("ERROR in answer:"))
            {
                ErrorData += (ethnicity + " for application " + application.Id + " and reference " + application.Reference) + "\n";
            }

            return ethnicity switch
            {
                PersonEthnicity.AsianIndian => "IND",
                PersonEthnicity.AsianPakistani => "PAK",
                PersonEthnicity.AsianBangladeshi => "BAN",
                PersonEthnicity.AsianChinese => "CHI",
                PersonEthnicity.AsianVietnamese => "VIE",
                PersonEthnicity.AsianOther => "AOA",
                PersonEthnicity.AsianNepali => "NEP",
                PersonEthnicity.AsianSriLankanOther => "SLO",
                PersonEthnicity.AsianSriLankanTamil => "SLT",
                PersonEthnicity.AsianSriLankanSinhalese => "SLS",

                PersonEthnicity.BlackCaribbean => "CAR",
                PersonEthnicity.BlackBritish => "BBR",
                PersonEthnicity.BlackCongolese => "CON",
                PersonEthnicity.Ghanaian => "GHA",
                PersonEthnicity.BlackGhanaian => "BGH",
                PersonEthnicity.BlackNigerian => "NIG",
                PersonEthnicity.BlackSomali => "SOM",
                PersonEthnicity.BlackAngolan => "BLA",
                PersonEthnicity.BlackSudanese => "BSU",
                PersonEthnicity.BlackAfricanOther => "OBA",
                PersonEthnicity.BlackSierraLeonean => "BSL",

                PersonEthnicity.WhiteAndBlackCaribbean => "WBC",
                PersonEthnicity.WhiteAndBlackAfrican => "WBA",
                PersonEthnicity.WhiteAndAsian => "WAS",
                PersonEthnicity.OtherMixedBackground => "AOM",

                PersonEthnicity.WhiteBritish => "WBR",
                PersonEthnicity.WhiteJewish => "JEW",
                PersonEthnicity.WhiteCharediJew => "CHJ",
                PersonEthnicity.WhiteOrthodoxJew => "OJE",
                PersonEthnicity.WhiteGreekCypriot => "GCY",
                PersonEthnicity.WhiteIrish => "IRI",
                PersonEthnicity.WhiteTurkishCypriot => "TCY",
                PersonEthnicity.WhiteOtherEasternEuropean => "WOE",
                PersonEthnicity.WhiteOtherWesternEuropean => "WWE",
                PersonEthnicity.WhiteGypsyOrIrishTraveller => "IRT",
                PersonEthnicity.WhiteOther => "AOW",
                PersonEthnicity.WhitePolish => "EEU",
                PersonEthnicity.WhiteTurkish => "TUR",
                PersonEthnicity.WhiteItalian => "WWE",
                PersonEthnicity.WhiteNorthAmerican => "AOW",
                PersonEthnicity.WhiteAustralianNewZealander => "AOW",

                PersonEthnicity.OtherKurdish => "KUR",
                PersonEthnicity.OtherEuropean => "OEU",
                PersonEthnicity.OtherGypsyRoma => "GRO",
                PersonEthnicity.OtherVietnamese => "VIE",
                PersonEthnicity.OtherThai => "THA",
                PersonEthnicity.OtherArab => "ARO",
                PersonEthnicity.OtherMalay => "MAL",
                PersonEthnicity.OtherIraqi => "IRQ",
                PersonEthnicity.OtherKorean => "KOR",
                PersonEthnicity.OtherAfghan => "AOO",
                PersonEthnicity.OtherLibyan => "LIB",
                PersonEthnicity.OtherYemeni => "YEM",
                PersonEthnicity.OtherIranian => "IRA",
                PersonEthnicity.OtherTurkish => "TUR",
                PersonEthnicity.OtherEgyptian => "EGY",
                PersonEthnicity.OtherFilipino => "FIL",
                PersonEthnicity.OtherJapanese => "JAP",
                PersonEthnicity.OtherLebanese => "LEB",
                PersonEthnicity.OtherMoroccan => "MOR",
                PersonEthnicity.OtherPolynesian => "POL",
                PersonEthnicity.OtherLatinSouthCentralAmerican => "LAT",
                PersonEthnicity.OtherCongolese => "CON",
                PersonEthnicity.Other => "AOO",
                _ => "PRE",
            };
        }
    }
}
