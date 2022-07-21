using System.Collections.Generic;

namespace HousingRegisterApi.V1.Boundary.Response
{
    public class ApplicationSearchPagedResponse
    {
        public List<ApplicationSearchResultResponse> Results { get; set; }

        public long TotalResults { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}
