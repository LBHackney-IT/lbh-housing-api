using HousingRegisterApi.V1.Gateways.Interfaces;
using HousingRegisterApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class ApplicationBreakdownByStatusUseCase : IApplicationBreakdownByStatusUseCase
    {
        private readonly ISearchGateway _search;

        public ApplicationBreakdownByStatusUseCase(ISearchGateway search)
        {
            _search = search;
        }

        public async Task<Dictionary<string, long>> Execute()
        {
            return await _search.GetStatusBreakdown().ConfigureAwait(false);
        }
    }
}
