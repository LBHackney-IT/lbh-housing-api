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

        public EvidenceApiGateway(HttpClient httpClient)
        {
            _client = httpClient;
            _client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("EVIDENCE_API_URL"));
        }

        public async Task<EvidenceRequestResponse> CreateEvidenceRequest(CreateEvidenceRequest request)
        {
            var uri = new Uri("api/v1/evidence_requests", UriKind.Relative);
            AddRequestHeaders(Environment.GetEnvironmentVariable("EVIDENCE_API_POST_EVIDENCE_REQUESTS_TOKEN"));

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
            AddRequestHeaders(Environment.GetEnvironmentVariable("EVIDENCE_API_GET_EVIDENCE_REQUESTS_TOKEN"));

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
            AddRequestHeaders(Environment.GetEnvironmentVariable("EVIDENCE_API_GET_EVIDENCE_REQUESTS_TOKEN"));

            var response = await _client.GetAsync(uri).ConfigureAwait(true);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new EvidenceApiException($"Incorrect status code returned: {response.StatusCode}");
            }

            return await response.Content.ReadAsAsync<EvidenceRequestResponse>().ConfigureAwait(true);
        }

        private void AddRequestHeaders(string authValue)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authValue);
            _client.DefaultRequestHeaders.Add("UserEmail", "housingregister@hackney.gov.uk");
        }
    }
}
