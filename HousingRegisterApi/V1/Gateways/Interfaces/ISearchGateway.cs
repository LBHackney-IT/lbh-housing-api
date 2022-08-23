using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Gateways.Interfaces
{
    public interface ISearchGateway
    {
        Task<ApplicationSearchPagedResult> SearchApplications(string queryPhrase, int pageNumber, int pageSize);

        Task<ApplicationSearchPagedResult> FilterApplications(SearchQueryParameter filterParameters);

        Task<Dictionary<string, long>> GetStatusBreakdown();

        Task<ApplicationSearchPagedResult> GetByBiddingNumber(long biddingNumber, int page, int pageSize);
    }
}
