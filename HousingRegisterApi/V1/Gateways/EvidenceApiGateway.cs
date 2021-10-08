using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Boundary.Response.Exceptions;

namespace HousingRegisterApi.V1.Gateways
{
    public class EvidenceApiGateway : IEvidenceApiGateway
    {
        private readonly HttpClient _client;
        private readonly ApiOptions _options;

        public EvidenceApiGateway(HttpClient httpClient, ApiOptions options)
        {
            _client = httpClient;
            _options = options;
            _client.BaseAddress = _options.EvidenceApiUrl;
        }

        public async Task<EvidenceRequestResponse> CreateEvidenceRequest(CreateEvidenceRequest request)
        {
            var uri = new Uri("api/v1/evidence_requests", UriKind.Relative);
            AddRequestHeaders(_options.EvidenceApiPostClaimsToken);

            var response = await _client.PostAsJsonAsync(uri, request).ConfigureAwait(true);
            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new EvidenceApiException($"Incorrect status code returned: {response.StatusCode}");
            }

            return await response.Content.ReadAsAsync<EvidenceRequestResponse>().ConfigureAwait(true);
        }

        public async Task<List<EvidenceRequestResponse>> GetEvidenceRequests(string team)
        {
            var uri = new Uri($"api/v1/evidence_requests?Team={team}", UriKind.Relative);
            AddRequestHeaders(_options.EvidenceApiGetClaimsToken);

            var response = await _client.GetAsync(uri).ConfigureAwait(true);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new EvidenceApiException($"Incorrect status code returned: {response.StatusCode}");
            }

            return await response.Content.ReadAsAsync<List<EvidenceRequestResponse>>().ConfigureAwait(true);
        }

        public async Task<EvidenceRequestResponse> GetEvidenceRequest(Guid id)
        {
            var uri = new Uri($"api/v1/evidence_requests/{id}", UriKind.Relative);
            AddRequestHeaders(_options.EvidenceApiGetClaimsToken);

            var response = await _client.GetAsync(uri).ConfigureAwait(true);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new EvidenceApiException($"Incorrect status code returned: {response.StatusCode}");
            }

            return await response.Content.ReadAsAsync<EvidenceRequestResponse>().ConfigureAwait(true);
        }

        private void AddRequestHeaders(string authValue)
        {
            var requestedBy = !string.IsNullOrEmpty(_options.EvidenceRequestedBy) ? _options.EvidenceRequestedBy : "housingregister@hackney.gov.uk";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authValue);
            _client.DefaultRequestHeaders.Add("UserEmail", requestedBy);
        }
    }
}
