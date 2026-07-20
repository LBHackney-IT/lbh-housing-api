using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1
{
    public class ApiOptions
    {
        public string HackneyJwtSecret { get; set; }
        public string NotifyApiKey { get; set; }
        public string NotifyTemplateVerifyCode { get; set; }
        public Uri EvidenceApiUrl { get; set; }
        public string EvidenceRequestedBy { get; set; }
        public string EvidenceApiGetClaimsToken { get; set; }
        public string EvidenceApiPostClaimsToken { get; set; }
        public Uri ActivityHistoryApiUrl { get; set; }

        /// <summary>
        /// Comma-separated auth email blocklist. Supports exact emails and domain
        /// wildcards such as *@prbly.win.
        /// </summary>
        public string BlockedAuthEmails { get; set; }

        public string AuthorisedAdminGroup { get; set; }

        public string AuthorisedManagerGroup { get; set; }

        public string AuthorisedOfficerGroup { get; set; }

        public string AuthorisedReadOnlyGroup { get; set; }

        public IEnumerable<string> GetStaffGroupsAllowedToCreateApplications()
        {
            return new[]
            {
                AuthorisedAdminGroup,
                AuthorisedManagerGroup,
                AuthorisedOfficerGroup,
            }.Where(group => !string.IsNullOrWhiteSpace(group));
        }

        public static ApiOptions FromEnv()
        {
            var evidenceApiUrl = Environment.GetEnvironmentVariable("EVIDENCE_API_URL");
            var activityApiUrl = Environment.GetEnvironmentVariable("ACTIVITY_HISTORY_API_URL");

            return new ApiOptions()
            {
                HackneyJwtSecret = Environment.GetEnvironmentVariable("HACKNEY_JWT_SECRET"),
                NotifyApiKey = Environment.GetEnvironmentVariable("NOTIFY_API_KEY"),
                NotifyTemplateVerifyCode = Environment.GetEnvironmentVariable("NOTIFY_TEMPLATE_VERIFY_CODE"),
                EvidenceApiUrl = evidenceApiUrl != null ? new Uri(evidenceApiUrl) : null,
                EvidenceRequestedBy = Environment.GetEnvironmentVariable("EVIDENCE_API_REQUESTED_BY"),
                EvidenceApiGetClaimsToken = Environment.GetEnvironmentVariable("EVIDENCE_API_GET_EVIDENCE_REQUESTS_TOKEN"),
                EvidenceApiPostClaimsToken = Environment.GetEnvironmentVariable("EVIDENCE_API_POST_EVIDENCE_REQUESTS_TOKEN"),
                ActivityHistoryApiUrl = activityApiUrl != null ? new Uri(activityApiUrl) : null,
                BlockedAuthEmails = Environment.GetEnvironmentVariable("BLOCKED_AUTH_EMAILS"),
                AuthorisedAdminGroup = Environment.GetEnvironmentVariable("AUTHORISED_ADMIN_GROUP"),
                AuthorisedManagerGroup = Environment.GetEnvironmentVariable("AUTHORISED_MANAGER_GROUP"),
                AuthorisedOfficerGroup = Environment.GetEnvironmentVariable("AUTHORISED_OFFICER_GROUP"),
                AuthorisedReadOnlyGroup = Environment.GetEnvironmentVariable("AUTHORISED_READONLY_GROUP"),
            };
        }
    }
}
