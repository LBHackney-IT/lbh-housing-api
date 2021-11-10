namespace HousingRegisterApi.V1.Infrastructure
{
    public static class CreateApplicationConstants
    {
        // JWT TOKEN
        public const string V1VERSION = "v1";
        public const string EVENTTYPE = "HousingApplicationCreatedEvent";
        public const string SOURCEDOMAIN = "HousingRegisterApplication";
        public const string SOURCESYSTEM = "HousingRegisterAPI";
    }

    public static class UpdateApplicationConstants
    {
        // JWT TOKEN
        public const string V1VERSION = "v1";
        public const string EVENTTYPE = "HousingApplicationUpdatedEvent";
        public const string SOURCEDOMAIN = "HousingRegisterApplication";
        public const string SOURCESYSTEM = "HousingRegisterAPI";
    }
}
