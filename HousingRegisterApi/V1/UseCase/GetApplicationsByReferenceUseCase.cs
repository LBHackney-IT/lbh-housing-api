using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Gateways.Interfaces;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using HousingRegisterApi.V1.Factories;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetApplicationsByReferenceUseCase : IGetApplicationsByReferenceUseCase
    {
        private readonly ISearchGateway _searchGateway;

        public GetApplicationsByReferenceUseCase(ISearchGateway searchGateway)
        {
            _searchGateway = searchGateway;
        }

        public async Task<ApplicationSearchPagedResponse> Execute(SearchQueryParameter searchParameters)
        {
            var searchResult = await _searchGateway.FilterApplications(searchParameters).ConfigureAwait(false);

            return searchResult.ToResponse();
        }
    }
}
