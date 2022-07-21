using System.Collections.Generic;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class ApplicationSearchPagedResult
    {
        public List<ApplicationSearchEntity> Results { get; set; }

        public long TotalResults { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}
