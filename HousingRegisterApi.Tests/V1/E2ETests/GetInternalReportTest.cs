using FluentAssertions;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.Report.Internal;
using HousingRegisterApi.V1.Factories;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HousingRegisterApi.Tests.V1.E2ETests
{
    //For guidance on writing integration tests see the wiki page https://github.com/LBHackney-IT/lbh-base-api/wiki/Writing-Integration-Tests    
    public class GetInternalReportTest : DynamoDbIntegrationTests<Startup>
    {
        private readonly ApplicationFixture _applicationFixture;

        public GetInternalReportTest()
        {
            _applicationFixture = new ApplicationFixture();
        }

        private async Task SetupTestData(Application entity)
        {
            await DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> GetTestRequestAsync(InternalReportRequest request)
        {
            var baseUrl = "api/v1/reporting/export";
            var reportType = $"reportType={Convert.ToInt32(request.ReportType)}";
            var startDate = $"startDate={request.StartDate.ToString("yyyy-MM-dd")}";
            var endDate = $"endDate={request.EndDate.ToString("yyyy-MM-dd")}";

            var uri = new Uri($"{baseUrl}?{reportType}&{startDate}&{endDate}", UriKind.Relative);
            return await Client.GetAsync(uri).ConfigureAwait(false);
        }

        [Test]
        public async Task GetCasesReportReturnsResponse()
        {
            // Arrange
            var entity = _applicationFixture.ConstructTestEntity();
            await SetupTestData(entity).ConfigureAwait(false);

            var request = new InternalReportRequest
            {
                ReportType = InternalReportType.CasesReport,
                StartDate = DateTime.Now.AddDays(-7).Date,
                EndDate = DateTime.Now.AddDays(7).Date
            };

            // Act            
            var response = await GetTestRequestAsync(request).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
