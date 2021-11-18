using FluentAssertions;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HousingRegisterApi.Tests.V1.E2ETests
{
    //For guidance on writing integration tests see the wiki page https://github.com/LBHackney-IT/lbh-base-api/wiki/Writing-Integration-Tests    
    public class GetNovaletExportTest : DynamoDbIntegrationTests<Startup>
    {
        private readonly ApplicationFixture _applicationFixture;

        public GetNovaletExportTest()
        {
            _applicationFixture = new ApplicationFixture();
        }

        private async Task SetupTestData(Application entity)
        {
            await DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> GetTestRequestAsync()
        {
            var uri = new Uri($"api/v1/reporting/novaletexport", UriKind.Relative);
            return await Client.GetAsync(uri).ConfigureAwait(false);
        }

        [Test]
        public async Task GetNovaletExportForApplicationReturnsResponse()
        {
            // Arrange
            var entity = _applicationFixture.ConstructTestEntity();
            entity.Status = ApplicationStatus.Active;
            await SetupTestData(entity).ConfigureAwait(false);

            // Act            
            var response = await GetTestRequestAsync().ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
