using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways.Interfaces;
using HousingRegisterApi.V1.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingRegisterApi.V1.Infrastructure.Search;
using HousingRegisterApi.V1.Boundary.Request;

namespace HousingRegisterApi.V1.Gateways
{
    public class SearchGateway : ISearchGateway
    {
        private readonly ILogger<SearchGateway> _logger;
        private ElasticClient _client;

        const string HousingRegisterReadAlias = "housing-register-applications";

        public SearchGateway(ILogger<SearchGateway> logger, IConfiguration configuration)
        {
            _logger = logger;
            _client = new ElasticClient(new Uri(configuration["SEARCHDOMAIN"]));

            _client.ConnectionSettings.IdProperties.Add(typeof(ApplicationSearchEntity), "ApplicationId");
        }

        public async Task<ApplicationSearchPagedResult> SearchApplications(string queryPhrase, int pageNumber, int pageSize = 10)
        {
            var simpleQueryStringSearch = await _client.SearchAsync<ApplicationSearchEntity>(s => s
                .Index(HousingRegisterReadAlias)
                .Query(topLevelQuery => topLevelQuery
                    .Bool(bq => bq
                        .Should(sq => sq
                            .Nested(nq => nq
                                .Path(np=>np.OtherMembers)
                                .Query(inq => inq
                                    .SimpleQueryString(isq => isq
                                        .Query(queryPhrase)
                                        .DefaultOperator(Operator.Or)
                                    )
                                )
                            )
                        )
                        .Should(sq => sq
                            .SimpleQueryString(isq => isq
                                .Query(queryPhrase)
                                .DefaultOperator(Operator.Or)
                            )
                        )
                    )
                )
                .Take(pageSize)
                .From(pageSize * pageNumber)
            ).ConfigureAwait(true);

            return simpleQueryStringSearch.ToPagedResult(pageNumber, pageSize);
        }

        public async Task<Dictionary<string, long>> GetStatusBreakdown()
        {
            var aggsResult = await _client.SearchAsync<ApplicationSearchEntity>(r => r
                .Aggregations(agg => agg
                    .Terms("status", ta => ta
                        .Field("status")
                    )
                )
                .Size(0)
                .Source(false)
            ).ConfigureAwait(true);

            Dictionary<string, long> result = new Dictionary<string, long>();

            var aggregate = aggsResult.Aggregations.Terms("status");

            foreach (var bucket in aggregate.Buckets)
            {
                result.Add(bucket.Key, bucket.DocCount ?? 0);
            }

            return result;
        }

        public async Task<ApplicationSearchPagedResult> FilterApplications(SearchQueryParameter filterParameters)
        {
            QueryContainer queryContainer = null;

            if (!string.IsNullOrWhiteSpace(filterParameters.Status))
            {
                queryContainer &= +Query<ApplicationSearchEntity>.Match(s => s.Field(f => f.Status).Query(filterParameters.Status));
            }

            if (!string.IsNullOrWhiteSpace(filterParameters.AssignedTo))
            {
                queryContainer &= +Query<ApplicationSearchEntity>.Match(s => s.Field(f => f.AssignedTo).Query(filterParameters.AssignedTo));
            }

            if (filterParameters.HasAssessment.HasValue)
            {
                queryContainer &= +Query<ApplicationSearchEntity>.Match(s => s.Field(f => f.HasAssessment).Query(filterParameters.HasAssessment.Value ? "true" : "false"));
            }

            if (!string.IsNullOrWhiteSpace(filterParameters.NationalInsurance))
            {
                queryContainer &= +Query<ApplicationSearchEntity>.Match(s => s.Field(f => f.NationalInsuranceNumber).Query(filterParameters.NationalInsurance));
            }

            if (!string.IsNullOrWhiteSpace(filterParameters.Reference))
            {
                queryContainer &= +Query<ApplicationSearchEntity>.Match(s => s.Field(f => f.Reference).Query(filterParameters.Reference));
            }

            SearchRequest<ApplicationSearchEntity> request = new SearchRequest<ApplicationSearchEntity>(HousingRegisterReadAlias)
            {
                Query = queryContainer,
                From = filterParameters.Page * filterParameters.PageSize,
                Size = filterParameters.PageSize,
                Sort = new List<ISort> { { new FieldSort { Field = new Field(filterParameters.OrderBy), Order = SortOrder.Descending } } }
            };

            var results = await _client.SearchAsync<ApplicationSearchEntity>(request).ConfigureAwait(true);

            return results.ToPagedResult(filterParameters.Page, filterParameters.PageSize);
        }
    }
}
