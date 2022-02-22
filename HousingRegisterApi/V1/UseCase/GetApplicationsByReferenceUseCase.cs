using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class GetApplicationsByReferenceUseCase : IGetApplicationsByReferenceUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly IPaginationHelper _paginationHelper;

        public GetApplicationsByReferenceUseCase(IApplicationApiGateway gateway, IPaginationHelper paginationHelper)
        {
            _gateway = gateway;
            _paginationHelper = paginationHelper;
        }

        public async Task<PaginatedApplicationListResponse> Execute(SearchQueryParameter searchParameters)
        {
            var (data, paginationToken) = await _gateway.GetApplicationsByReferenceAsync(searchParameters).ConfigureAwait(false);

            return _paginationHelper.BuildResponse(searchParameters, data, paginationToken);
        }
    }
}
