using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Gateways.Interfaces;
using HousingRegisterApi.V1.UseCase.Interfaces;
using HousingRegisterApi.V1.Factories;
using System.Threading.Tasks;

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
            var searchResults = await _search.SearchApplications(request.QueryString, request.Page, request.PageSize).ConfigureAwait(true);

            return searchResults.ToResponse();
        }
    }
}
