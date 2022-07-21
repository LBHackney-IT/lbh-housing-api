using Nest;
using System.Linq;

namespace HousingRegisterApi.V1.Infrastructure.Search
{
    public static class SearchResultExtensions
    {
        public static ApplicationSearchPagedResult ToPagedResult(this ISearchResponse<ApplicationSearchEntity> results, int pageNumber, int pageSize)
        {
            return new ApplicationSearchPagedResult
            {
                TotalResults = results.Total,
                Page = pageNumber,
                PageSize = pageSize,
                Results = results.Hits.Select(r => r.Source).ToList()
            };
        }
    }
}
