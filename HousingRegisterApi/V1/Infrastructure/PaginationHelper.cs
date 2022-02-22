using Hackney.Core.DynamoDb;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class PaginationHelper : IPaginationHelper
    {
        public PaginatedApplicationListResponse BuildResponse(SearchQueryParameter searchParameters, IEnumerable<Application> results, string paginationToken)
        {
            return new PaginatedApplicationListResponse
            {
                Results = results.ToResponse(),
                PaginationToken = paginationToken,
            };
        }
    }
}
