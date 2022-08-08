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
        private readonly IApplicationApiGateway _gateway;
        private readonly IPaginationHelper _paginationHelper;
        private readonly ISearchGateway _searchGateway;

        public GetAllApplicationsUseCase(IApplicationApiGateway gateway, IPaginationHelper paginationHelper, ISearchGateway searchGateway)
        {
            _gateway = gateway;
            _paginationHelper = paginationHelper;
            _searchGateway = searchGateway;
        }

        public async Task<ApplicationSearchPagedResponse> Execute(SearchQueryParameter searchParameters)
        {
            var searchResponse = await _searchGateway.FilterApplications(searchParameters).ConfigureAwait(false);

            return searchResponse.ToResponse();
        }
    }
}
