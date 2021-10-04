namespace HousingRegisterApi.V1.Boundary.Request
{
    public class PaginatedDataRequestBase
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
