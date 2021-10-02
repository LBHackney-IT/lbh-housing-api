using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Infrastructure
{
    public interface IPaginationHelper
    {
        IEnumerable<Application> PageData(IEnumerable<Application> data, int page, int itemsPerPage);

        IEnumerable<Application> OrderData(IEnumerable<Application> data, string orderBy);

        PaginatedApplicationListResponse BuildResponse(SearchQueryParameter searchParameters, IEnumerable<Application> data, int totalItems);
    }
}
