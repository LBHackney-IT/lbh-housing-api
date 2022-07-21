namespace HousingRegisterApi.V1.Boundary.Request
{
    public class ApplicationSearchRequest
    {
        public string QueryString { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

    }
}
