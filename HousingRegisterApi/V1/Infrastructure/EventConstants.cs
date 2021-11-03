namespace HousingRegisterApi.V1.Infrastructure
{
    public static class CreateApplicationConstants
    {
        // JWT TOKEN
        public const string V1VERSION = "v1";
        public const string EVENTTYPE = "ApplicationCreatedEvent";
        public const string SOURCEDOMAIN = "Application";
        public const string SOURCESYSTEM = "HousingRegisterAPI";
    }

    public static class UpdateApplicationConstants
    {
        // JWT TOKEN
        public const string V1VERSION = "v1";
        public const string EVENTTYPE = "ApplicationUpdatedEvent";
        public const string SOURCEDOMAIN = "Application";
        public const string SOURCESYSTEM = "HousingRegisterAPI";
    }
}
