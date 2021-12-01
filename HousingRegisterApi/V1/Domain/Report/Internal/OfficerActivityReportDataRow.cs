using Hackney.Shared.ActivityHistory.Boundary.Response;
using System.ComponentModel;

namespace HousingRegisterApi.V1.Domain.Report
{
    public class OfficerActivityReportDataRow
    {
        [Description("Officer")]
        public string Officer { get; set; }

        [Description("Event type")]
        public string ActivityType { get; set; }

        [Description("Date")]
        public string ActivityDate { get; set; }

        public OfficerActivityReportDataRow(ActivityHistoryResponseObject activity)
        {
            Officer = activity.AuthorDetails?.FullName;
            ActivityDate = activity.CreatedAt.ToString();
            activity.NewData.TryGetValue("_activityType", out object activityType);
            ActivityType = activityType?.ToString();
        }
    }
}
