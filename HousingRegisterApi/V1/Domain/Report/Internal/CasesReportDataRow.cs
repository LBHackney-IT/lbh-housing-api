using System;
using System.ComponentModel;
using System.Linq;

namespace HousingRegisterApi.V1.Domain.Report
{
    public class CasesReportDataRow
    {
        [Description("Case number")]
        public string CaseNumber { get; set; }

        [Description("Contact name")]
        public string Name { get; set; }

        [Description("Contact email")]
        public string Email { get; set; }

        [Description("Contact phone")]
        public string ContactTelephone { get; set; }

        [Description("Drafted date")]
        public string CreatedAt { get; set; }

        [Description("Submission date")]
        public string SubmittedOn { get; set; }

        [Description("Status")]
        public string Status { get; set; }

        [Description("Reason")]
        public string Reason { get; set; }

        [Description("Application date")]
        public string EffectiveOn { get; set; }

        [Description("All information received")]
        public string AllInformationReceived { get; set; }

        [Description("Calculated bedroom need")]
        public string CalculatedBedroomNeed { get; set; }

        [Description("Assessed bedroom need")]
        public string BedroomNeed { get; set; }

        [Description("Band")]
        public string Band { get; set; }

        [Description("Bidding number")]
        public string BiddingNumber { get; set; }

        [Description("Tenure (current accommodation)")]
        public string Tenure { get; set; }

        [Description("Count of household members")]
        public string HouseholdCount { get; set; }

        [Description("Living situation")]
        public string LivingStatus { get; set; }

        [Description("Money")]
        public string Money { get; set; }

        [Description("Health")]
        public string Health { get; set; }      

        public CasesReportDataRow(Application application)
        {
            CaseNumber = application.Reference;
            Name = application.MainApplicant?.Person?.FullName;
            Email = application.MainApplicant?.ContactInformation?.EmailAddress;
            ContactTelephone = application.MainApplicant?.ContactInformation?.PhoneNumber;
            CreatedAt = FormatDate(application.CreatedAt);
            SubmittedOn = FormatDate(application.Assessment?.EffectiveDate);

            Status = application.Status;
            Reason = application.Assessment?.Reason;
            EffectiveOn = FormatDate(application.Assessment?.EffectiveDate);
            AllInformationReceived = FormatDate(application.Assessment?.InformationReceivedDate);
            CalculatedBedroomNeed = application.CalculatedBedroomNeed.ToString();
            BedroomNeed = application.Assessment?.BedroomNeed.ToString();
            Band = application.Assessment?.Band;
          
            BiddingNumber = application.Assessment?.BiddingNumber;
            Tenure = application.MainApplicant.Questions.GetAnswer("current-accommodation");
            HouseholdCount = (1 + application.OtherMembers.Count()).ToString();
            LivingStatus = application.MainApplicant.Questions.GetAnswer("current-accommodation/living-situation");
            Money = application.MainApplicant.Questions.GetAnswer("income-savings");
            Health = application.MainApplicant.MedicalNeed?.Outcome;
        }

        private static string FormatDate(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                return dateTime.Value.ToString("dd/MM/yyyy");
            }

            return null;
        }       
    }
}
