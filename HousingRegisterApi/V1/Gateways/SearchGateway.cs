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
using System.Text;

namespace HousingRegisterApi.V1.Gateways
{
    public class SearchGateway : ISearchGateway
    {
        private readonly ILogger<SearchGateway> _logger;
        private ElasticClient _client;
        private ConnectionSettings _connectionSettings;

        const string HousingRegisterReadAlias = "housing-register-applications";

        public SearchGateway(ILogger<SearchGateway> logger, IConfiguration configuration)
        {
            _logger = logger;
            var searchDomainUri = new Uri(configuration["SEARCHDOMAIN"]);
            _connectionSettings = new ConnectionSettings(searchDomainUri);

#if DEBUG
            //When debugging locally, dont check for valid SSL to the search domain
            _connectionSettings.ServerCertificateValidationCallback((o, cert, chain, sslErrors) => true);
            _connectionSettings.EnableDebugMode();
#endif
            _client = new ElasticClient(_connectionSettings);

            _client.ConnectionSettings.IdProperties.Add(typeof(ApplicationSearchEntity), "ApplicationId");
        }

        public async Task<ApplicationSearchPagedResult> SearchApplications(string queryPhrase, int pageNumber, int pageSize = 10)
        {
            var baseSearch = ConstructApplicationSearch(queryPhrase, pageNumber, pageSize);

            var simpleQueryStringSearch = await _client.SearchAsync<ApplicationSearchEntity>(s => baseSearch).ConfigureAwait(false);

            string queryJson = System.Text.Encoding.UTF8.GetString(simpleQueryStringSearch.ApiCall.RequestBodyInBytes);

            return simpleQueryStringSearch.ToPagedResult(pageNumber, pageSize);
        }

        public static SearchDescriptor<ApplicationSearchEntity> ConstructApplicationSearch(string queryPhrase, int pageNumber, int pageSize)
        {
            var structuedQuery = ParseQuery(queryPhrase);

            var offsetPageNumber = Math.Max(1, pageNumber);

            var topLevelQuery = new QueryContainerDescriptor<ApplicationSearchEntity>()
                .SimpleQueryString(isq => isq
                                    .Query(structuedQuery.GetSimpleQueryStringWithFuzziness())
                                    .DefaultOperator(Operator.Or)
                                    .Fields(f => f.Fields(f => f.Surname, f => f.FirstName, f => f.MiddleName, f => f.ApplicationId))
                                );

            var nestedDocQuery = new QueryContainerDescriptor<ApplicationOtherMembersSearchEntity>()
                .SimpleQueryString(isq => isq
                                    .Query(structuedQuery.GetSimpleQueryStringWithFuzziness())
                                    .DefaultOperator(Operator.Or)
                                    .Fields(f => f.Fields(f => f.Surname, f => f.FirstName, f => f.MiddleName))
                                );

            //Add wildcards on detected entities
            foreach (var partialNino in structuedQuery.NINOs)
            {
                topLevelQuery |= Query<ApplicationSearchEntity>.Wildcard(w => w.NationalInsuranceNumber, $"{partialNino}*");
                nestedDocQuery |= Query<ApplicationOtherMembersSearchEntity>.Wildcard(w => w.NationalInsuranceNumber, $"{partialNino}*");
            }

            foreach (var partialReference in structuedQuery.ReferenceNumbers)
            {
                topLevelQuery |= Query<ApplicationSearchEntity>.Wildcard(w => w.Reference, $"{partialReference}{(partialReference.Length == 10 ? "" : "*")}");
            }

            foreach (var date in structuedQuery.Dates)
            {
                topLevelQuery |= Query<ApplicationSearchEntity>.Term(f => f.DateOfBirth, date);
                nestedDocQuery |= Query<ApplicationOtherMembersSearchEntity>.Term(f => f.DateOfBirth, date);
            }

            foreach (var biddingNumber in structuedQuery.BiddingNumbers)
            {
                topLevelQuery |= Query<ApplicationSearchEntity>.Term(f => f.BiddingNumber, biddingNumber);
            }

            SearchDescriptor<ApplicationSearchEntity> baseSearch = new SearchDescriptor<ApplicationSearchEntity>()
            .Index(HousingRegisterReadAlias)
            .Query(tlq => tlq
                .Bool(bq => bq
                    .Should(sq => sq
                        .Nested(nq => nq
                            .Path(np => np.OtherMembers)
                            .Query(inq => nestedDocQuery)
                        ),
                        sq => topLevelQuery
                    )
                 )
            )
            .Take(pageSize)
            .From(pageSize * (offsetPageNumber - 1));
            return baseSearch;
        }

        public async Task<Dictionary<string, long>> GetStatusBreakdown()
        {
            var aggsResult = await _client.SearchAsync<ApplicationSearchEntity>(r => r
                .Index(HousingRegisterReadAlias)
                .Aggregations(agg => agg
                    .Terms("status", ta => ta
                        .Field("status")
                        .Size(100)
                    )
                )
                .Size(0)
                .Source(false)
            ).ConfigureAwait(false);

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
            int offsetPageNumber = Math.Max(1, filterParameters.Page);

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
                From = filterParameters.PageSize * (offsetPageNumber - 1),
                Size = filterParameters.PageSize,
                Sort = new List<ISort> { { new FieldSort { Field = new Field(GetKeywordFieldName(filterParameters.OrderBy)), Order = SortOrder.Descending } } }
            };

            var results = await _client.SearchAsync<ApplicationSearchEntity>(request).ConfigureAwait(false);

            return results.ToPagedResult(filterParameters.Page, filterParameters.PageSize);
        }

        private static string GetKeywordFieldName(string orderBy)
        {

            switch (orderBy?.ToLower()?.Trim())
            {
                case "firstname":
                case "surname":
                case "nationalinsurancenumber":
                case "emailaddress":
                case "phonenumber":
                case "reference":
                    return $"{orderBy}.keyword";
                default:
                    return orderBy ?? "surname.keyword";
            }

        }



        public static ApplicationSearchSemiStructuredQuery ParseQuery(string inputQuery)
        {
            if (string.IsNullOrWhiteSpace(inputQuery)) return ApplicationSearchSemiStructuredQuery.Empty;

            ApplicationSearchQueryParser parser = new ApplicationSearchQueryParser(inputQuery)
                .CaptureDates()
                .CaptureReferenceNumbers(false)
                .CaptureNINO(false)
                .CaptureFullBiddingNumbers(false)
                .RemoveMatched();

            return parser.Query;
        }
    }
}
