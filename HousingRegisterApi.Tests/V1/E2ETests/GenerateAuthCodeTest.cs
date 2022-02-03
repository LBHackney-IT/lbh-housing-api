using FluentAssertions;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
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
    public class GenerateAuthCodeTest : DynamoDbIntegrationTests<Startup>
    {
        private readonly AuthFixture _authFixture;
        public GenerateAuthCodeTest()
        {
            _authFixture = new AuthFixture();
        }

        private async Task<HttpResponseMessage> PostTestRequestAsync(string input)
        {
            using var data = new StringContent(input, Encoding.UTF8, "application/json");
            var uri = new Uri($"api/v1/auth/generate", UriKind.Relative);
            return await Client.PostAsync(uri, data).ConfigureAwait(false);
        }

        [Test]
        [Ignore("Ignore for S3")]
        public async Task GenerateAuthCodeReturnsValidResponse()
        {
            // Arrange
            var request = _authFixture.ConstructCreateAuthRequestRequest();
            var json = JsonConvert.SerializeObject(request);

            // Act
            var response = await PostTestRequestAsync(json).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
