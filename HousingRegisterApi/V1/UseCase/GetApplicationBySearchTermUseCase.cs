using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetApplicationBySearchTermUseCase : IGetApplicationBySearchTermUseCase
    {
        private readonly IApplicationApiGateway _gateway;

        public GetApplicationBySearchTermUseCase(IApplicationApiGateway gateway)
        {
            _gateway = gateway;
        }

        public PaginatedApplicationListResponse Execute(SearchApplicationRequest searchParameters)
        {
            var totalItems = _gateway.GetAllBySearchTerm(searchParameters);

            var updateditems = OrderData(totalItems, searchParameters.OrderBy);
            updateditems = PageData(updateditems, searchParameters.Page, searchParameters.NumberOfItemsPerPage);

            return BuildResponse(searchParameters, updateditems.ToResponse(), totalItems.Count());
        }

        private static IEnumerable<Application> PageData(IEnumerable<Application> data, int page, int itemsPerPage)
        {
            return data.Skip(itemsPerPage * (page - 1)).Take(itemsPerPage);
        }

        private static IEnumerable<Application> OrderData(IEnumerable<Application> data, string orderBy)
        {
            return orderBy switch
            {
                "created_at" => data.OrderBy(x => x.CreatedAt).ToList(),
                "created_at_desc" => data.OrderByDescending(x => x.CreatedAt).ToList(),
                "surname" => data.OrderBy(x => x.MainApplicant.Person.Surname).ToList(),
                "surname_desc" => data.OrderByDescending(x => x.MainApplicant.Person.Surname).ToList(),
                "submited_at" => data.OrderBy(x => x.SubmittedAt).ToList(),
                "submitted_at_desc" => data.OrderByDescending(x => x.SubmittedAt).ToList(),
                _ => data.OrderBy(x => x.SubmittedAt).ToList()
            };
        }

        private static PaginatedApplicationListResponse BuildResponse(SearchApplicationRequest searchParameters, List<ApplicationResponse> results, int totalItems)
        {
            return new PaginatedApplicationListResponse
            {
                NumberOfItemsPerPage = searchParameters.NumberOfItemsPerPage,
                Page = searchParameters.Page,
                Results = results,
                TotalItems = totalItems
            };
        }
    }
}
