using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
using HousingRegisterApi.V1.Boundary.Response;
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

        private async Task<HttpResponseMessage> PostTestRequestAsync(string input)
        {
            using var data = new StringContent(input, Encoding.UTF8, "application/json");
            var uri = new Uri($"api/v1/applications/bedrooms", UriKind.Relative);
            return await Client.PostAsync(uri, data).ConfigureAwait(false);
        }

        [Test]
        public async Task ValidRequestToCalculateBedroomsReturnsOk()
        {
            // Arrange
            var request = _applicationFixture.ConstructCalculateBedroomsRequest();
            var json = JsonConvert.SerializeObject(request);

            // Act
            var response = await PostTestRequestAsync(json).ConfigureAwait(false);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var responseValue = JsonConvert.DeserializeObject<SimpleTypeResponse<int>>(responseContent);

            // Assert
            responseValue.Value.Should().BeGreaterOrEqualTo(0);
        }

        [Test]
        public async Task InvalildRequestToCalculateBedroomsReturnsBadRequest()
        {
            // Act
            var response = await PostTestRequestAsync("test").ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
