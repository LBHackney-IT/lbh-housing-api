using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Gateways.Interfaces;
using HousingRegisterApi.V1.UseCase.Interfaces;
using HousingRegisterApi.V1.Factories;
using System.Threading.Tasks;
using System;

namespace HousingRegisterApi.V1.UseCase
{
    public class SearchApplicationsUseCase : ISearchApplicationUseCase
    {
        private readonly ISearchGateway _search;

        public SearchApplicationsUseCase(ISearchGateway search)
        {
            _search = search;
        }

        public async Task<ApplicationSearchPagedResponse> Execute(ApplicationSearchRequest request)
        {
            //Paginating fix for front end - page 1 is the first set of results.
            int page = Math.Max(1, request.Page);

            var searchResults = await _search.SearchApplications(request.QueryString, page - 1, request.PageSize).ConfigureAwait(false);

            return searchResults.ToResponse();
        }
    }
}
