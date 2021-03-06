using FluentAssertions;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.Report.Internal;
using HousingRegisterApi.V1.Factories;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
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

        private async Task<HttpResponseMessage> PostTestRequestAsync(InternalReportRequest request)
        {
            var stringRequest = JsonConvert.SerializeObject(request);
            using var data = new StringContent(stringRequest, Encoding.UTF8, "application/json");
            var uri = new Uri($"api/v1/reporting/export", UriKind.Relative);
            return await Client.PostAsync(uri, data).ConfigureAwait(false);
        }

        [Test]
        [Ignore("Ignore for S3")]
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
            var response = await PostTestRequestAsync(request).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        [Ignore("Ignore for S3")]
        public async Task GetPeopleReportReturnsResponse()
        {
            // Arrange
            var entity = _applicationFixture.ConstructTestEntity();
            await SetupTestData(entity).ConfigureAwait(false);

            var request = new InternalReportRequest
            {
                ReportType = InternalReportType.PeopleReport,
                StartDate = DateTime.Now.AddDays(-7).Date,
                EndDate = DateTime.Now.AddDays(7).Date
            };

            // Act            
            var response = await PostTestRequestAsync(request).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        [Ignore("Ignore for S3")]
        public async Task GetCasesActivityReportReturnsResponse()
        {
            // Arrange
            var entity = _applicationFixture.ConstructTestEntity();
            await SetupTestData(entity).ConfigureAwait(false);

            var request = new InternalReportRequest
            {
                ReportType = InternalReportType.CaseActivityReport,
                StartDate = DateTime.Now.AddDays(-7).Date,
                EndDate = DateTime.Now.AddDays(7).Date
            };

            // Act            
            var response = await PostTestRequestAsync(request).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        [Ignore("Ignore for S3")]
        public async Task GetOfficerActivityReportReturnsResponse()
        {
            // Arrange
            var entity = _applicationFixture.ConstructTestEntity();
            await SetupTestData(entity).ConfigureAwait(false);

            var request = new InternalReportRequest
            {
                ReportType = InternalReportType.OfficerActivityReport,
                StartDate = DateTime.Now.AddDays(-7).Date,
                EndDate = DateTime.Now.AddDays(7).Date
            };

            // Act            
            var response = await PostTestRequestAsync(request).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
