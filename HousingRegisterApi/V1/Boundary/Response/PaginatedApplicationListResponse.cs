namespace HousingRegisterApi.V1.Boundary.Response
{
    public class PaginatedApplicationListResponse : ApplicationList
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalNumberOfPages { get; set; }
        public int PageStartOffSet { get; set; }
        public int PageEndOffSet { get; set; }
    }
}
