using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System.Linq;

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

        public PaginatedApplicationListResponse Execute(SearchQueryParameter searchParameters)
        {
            var totalItems = _gateway.GetApplications(searchParameters);
            return _paginationHelper.BuildResponse(searchParameters, totalItems, totalItems.Count());
        }
    }
}
