namespace HousingRegisterApi.V1.Boundary.Response
{
    public class PaginatedApplicationListResponse : ApplicationList
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int NumberOfItemsPerPage { get; set; }
    }
}
