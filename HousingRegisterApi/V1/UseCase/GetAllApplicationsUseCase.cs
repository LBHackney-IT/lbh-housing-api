using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetAllApplicationsUseCase : IGetAllApplicationsUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly IPaginationHelper _paginationHelper;

        public GetAllApplicationsUseCase(IApplicationApiGateway gateway, IPaginationHelper paginationHelper)
        {
            _gateway = gateway;
            _paginationHelper = paginationHelper;
        }

        public async Task<PaginatedApplicationListResponse> Execute(SearchQueryParameter searchParameters)
        {
            var totalItems = await _gateway.GetApplicationsAsync(searchParameters).ConfigureAwait(false);
            
            return _paginationHelper.BuildResponse(searchParameters, totalItems.Results, totalItems.Results.Count, totalItems.PaginationDetails.NextToken);
        }
    }
}
