using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;

namespace HousingRegisterApi.V1.Gateways
{
    public interface IEvidenceApiGateway
    {
        public Task<EvidenceRequestResponse> CreateEvidenceRequest(CreateEvidenceRequest request);

        Task<List<EvidenceRequestResponse>> GetEvidenceRequests(string team);

        Task<EvidenceRequestResponse> GetEvidenceRequest(Guid id);
    }
}
