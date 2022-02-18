namespace HousingRegisterApi.V1.Boundary.Response
{
    public class PaginatedApplicationListResponse : ApplicationList
    {
        public string PaginationToken { get; set; }
    }
}
