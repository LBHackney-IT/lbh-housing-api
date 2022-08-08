using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways.Interfaces;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetAllApplicationsByStatusUseCase : IGetAllApplicationsByStatusUseCase
    {
        private readonly ISearchGateway _searchGateway;

        public GetAllApplicationsByStatusUseCase( ISearchGateway searchGateway)
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
