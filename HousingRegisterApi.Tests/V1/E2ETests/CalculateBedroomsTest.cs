using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.E2ETests
{
    //For guidance on writing integration tests see the wiki page https://github.com/LBHackney-IT/lbh-base-api/wiki/Writing-Integration-Tests
    public class CalculateBedroomsTest : DynamoDbIntegrationTests<Startup>
    {
        private readonly ApplicationFixture _applicationFixture;

        public CalculateBedroomsTest()
        {
            _applicationFixture = new ApplicationFixture();
        }

        private async Task SetupTestData(Application entity)
        {
            await DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> GetTestRequestAsync(Guid id)
        {
            var uri = new Uri($"api/v1/applications/{id}/bedrooms", UriKind.Relative);
            return await Client.GetAsync(uri).ConfigureAwait(false);
        }

        [Test]
        [Ignore("Ignore for S3")]
        public async Task ValidRequestToCalculateBedroomsReturnsOk()
        {
            // Arrange
            var entity = _applicationFixture.ConstructTestEntity();
            await SetupTestData(entity).ConfigureAwait(false);

            // Act
            var response = await GetTestRequestAsync(entity.Id).ConfigureAwait(false);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var responseValue = JsonConvert.DeserializeObject<SimpleTypeResponse<int>>(responseContent);

            // Assert
            responseValue.Value.Should().BeGreaterOrEqualTo(0);
        }

        [Test]
        [Ignore("Ignore for S3")]
        public async Task InvalildRequestToCalculateBedroomsReturnsNotFound()
        {
            var id = Guid.NewGuid();
            var response = await GetTestRequestAsync(id).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
