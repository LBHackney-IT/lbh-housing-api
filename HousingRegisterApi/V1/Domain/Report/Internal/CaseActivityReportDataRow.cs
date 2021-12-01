using Hackney.Shared.ActivityHistory.Boundary.Response;
using System;
using System.ComponentModel;
using System.Linq;

namespace HousingRegisterApi.V1.Domain.Report
{
    public class CaseActivityReportDataRow
    {
        [Description("Case number")]
        public string CaseNumber { get; set; }

        [Description("Old Date")]
        public string OldData { get; set; }

        [Description("Change")]
        public string Change { get; set; }

        [Description("Officer")]
        public string Officer { get; set; }

        [Description("Date")]
        public string ActivityDateTime { get; set; }

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

        public CaseActivityReportDataRow(ActivityHistoryResponseObject activity, Application application)
        {
            CaseNumber = application.Reference;
            OldData = activity.OldData != null ? string.Join(",", activity.OldData.Select(x => $"{x.Key} : {x.Value}")) : "";
            Change = activity.NewData != null ? string.Join(",", activity.NewData.Select(x => $"{x.Key} : {x.Value}")) : "";
            Officer = activity.AuthorDetails?.FullName;
            ActivityDateTime = activity.CreatedAt.ToString();

            Status = application.Status;
            Reason = application.Assessment?.Reason;
            EffectiveOn = FormatDate(application.Assessment?.EffectiveDate);
            AllInformationReceived = FormatDate(application.Assessment?.InformationReceivedDate);
            CalculatedBedroomNeed = application.CalculatedBedroomNeed.ToString();
            BedroomNeed = application.Assessment?.BedroomNeed.ToString();
            Band = application.Assessment?.Band;
            BiddingNumber = application.Assessment?.BiddingNumber;
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
