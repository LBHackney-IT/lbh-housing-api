using System;
using System.ComponentModel;
using System.Linq;

namespace HousingRegisterApi.V1.Domain.Report
{
    public class PeopleReportDataRow
    {
        [Description("Case number")]
        public string CaseNumber { get; set; }

        [Description("Application Id")]
        public string ApplicationId { get; set; }

        [Description("Name")]
        public string Name { get; set; }

        [Description("Main applicant")]
        public string IsMainApplicant { get; set; }

        [Description("Date of birth")]
        public string DateOfBirth { get; set; }

        [Description("Gender")]
        public string Gender { get; set; }

        [Description("Age")]
        public string Age { get; set; }

        [Description("Medical")]
        public string Medical { get; set; }

        [Description("Health")]
        public string Health { get; set; }

        [Description("Tenure (current accommodation)")]
        public string Tenure { get; set; }

        [Description("Drafted date")]
        public string CreatedAt { get; set; }

        [Description("Submission date")]
        public string SubmittedOn { get; set; }

        [Description("Application date")]
        public string EffectiveOn { get; set; }

        public PeopleReportDataRow(Applicant applicant, Application application)
        {
            CaseNumber = application.Reference;
            ApplicationId = application.Id.ToString();
            Name = applicant.Person?.FullName;
            IsMainApplicant = (application.MainApplicant.Person?.Id == applicant.Person?.Id).ToString();
            DateOfBirth = FormatDate(applicant.Person?.DateOfBirth);
            Gender = applicant.Person?.Gender;
            Age = applicant.Person?.Age.ToString();
            Medical = applicant.RequiresMedical.ToString();
            Health = applicant.MedicalNeed?.Outcome;
            Tenure = applicant.Questions.GetAnswer("current-accommodation");

            CreatedAt = FormatDate(application.CreatedAt);
            SubmittedOn = FormatDate(application.SubmittedAt);
            EffectiveOn = FormatDate(application.Assessment?.EffectiveDate);
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
