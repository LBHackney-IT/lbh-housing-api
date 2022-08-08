using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Gateways.Interfaces;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using HousingRegisterApi.V1.Factories;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetAllApplicationsUseCase : IGetAllApplicationsUseCase
    {
        private readonly ISearchGateway _searchGateway;

        public GetAllApplicationsUseCase(ISearchGateway searchGateway)
        {
            _searchGateway = searchGateway;
        }

        public async Task<ApplicationSearchPagedResponse> Execute(SearchQueryParameter searchParameters)
        {
            var searchResponse = await _searchGateway.FilterApplications(searchParameters).ConfigureAwait(false);

            return searchResponse.ToResponse();
        }
    }
}
