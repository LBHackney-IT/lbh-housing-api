using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Infrastructure
{
    public interface IPaginationHelper
    {
        PaginatedApplicationListResponse BuildResponse(SearchQueryParameter searchParameters, IEnumerable<Application> results, string paginationToken);
    }
}
