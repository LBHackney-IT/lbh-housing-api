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
        public IEnumerable<Application> PageData(IEnumerable<Application> data, int page, int itemsPerPage)
        {
            return data.Skip(itemsPerPage * (page - 1)).Take(itemsPerPage);
        }

        public IEnumerable<Application> OrderData(IEnumerable<Application> data, string orderBy)
        {
            return orderBy switch
            {
                "created_at" => data.OrderBy(x => x.CreatedAt).ToList(),
                "created_at_desc" => data.OrderByDescending(x => x.CreatedAt).ToList(),
                "surname" => data.OrderBy(x => x.MainApplicant.Person.Surname).ToList(),
                "surname_desc" => data.OrderByDescending(x => x.MainApplicant.Person.Surname).ToList(),
                "submitted_at" => data.OrderBy(x => x.SubmittedAt).ToList(),
                "submitted_at_desc" => data.OrderByDescending(x => x.SubmittedAt).ToList(),
                _ => data.OrderByDescending(x => x.SubmittedAt).ToList()
            };
        }

        public PaginatedApplicationListResponse BuildResponse(SearchQueryParameter searchParameters, IEnumerable<Application> data, int totalItems, string paginationToken)
        {
            var results = OrderData(data, searchParameters.OrderBy);
            results = PageData(results, searchParameters.Page, searchParameters.PageSize);

            var pageStartItem = (searchParameters.Page - 1) * searchParameters.PageSize + 1;
            var pageEndItem = totalItems;

            if (searchParameters.PageSize < totalItems)
            {
                pageEndItem = searchParameters.PageSize * searchParameters.Page;

                if (pageEndItem > totalItems)
                {
                    pageEndItem = totalItems;
                }
            }

            var pag = PaginationDetails.EncodeToken(searchParameters.PaginationToken);
            var pagDec = PaginationDetails.DecodeToken(pag);

            return new PaginatedApplicationListResponse
            {
                PageSize = searchParameters.PageSize,
                Page = searchParameters.Page,
                Results = results.ToResponse(),
                TotalItems = totalItems,
                TotalNumberOfPages = (int) Math.Ceiling((decimal) totalItems / searchParameters.PageSize),
                PageStartOffSet = pageStartItem,
                PageEndOffSet = pageEndItem,
                PaginationToken = paginationToken,
            };
        }
    }
}
