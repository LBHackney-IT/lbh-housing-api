namespace HousingRegisterApi.V1.Domain
{
    public static class ApplicationStatus
    {
        public static readonly string Verification = "Verification";
        public static readonly string New = "New";
        public static readonly string Submitted = "Submitted";
        public static readonly string Active = "Active";
        public static readonly string Pending = "Pending";
        public static readonly string Referred = "Referred";
        public static readonly string Rejected = "Rejected";
        public static readonly string Disqualified = "Disqualified";
        public static readonly string Cancelled = "Cancelled";
        public static readonly string Suspended = "Suspended";
        public static readonly string Housed = "Housed";
        public static readonly string ActiveUnderAppeal = "ActiveUnderAppeal";
        public static readonly string InactiveUnderAppeal = "InactiveUnderAppeal";
    }
}
