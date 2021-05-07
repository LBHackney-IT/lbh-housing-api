using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.E2ETests
{
    //For guidance on writing integration tests see the wiki page https://github.com/LBHackney-IT/lbh-base-api/wiki/Writing-Integration-Tests    
    public class CreateNewApplicationTest : DynamoDbIntegrationTests<Startup>
    {
        private readonly Fixture _fixture = new Fixture();

        /// <summary>
        /// Method to construct a test request that can be used in a test
        /// </summary>        
        /// <returns></returns>
        private CreateApplicationRequest ConstructTestRequest()
        {
            var entity = _fixture.Create<CreateApplicationRequest>();
            return entity;
        }

        private async Task<HttpResponseMessage> PostTestRequestAsync(string input)
        {
            using var data = new StringContent(input, Encoding.UTF8, "application/json");
            var uri = new Uri($"api/v1/applications/", UriKind.Relative);
            return await Client.PostAsync(uri, data).ConfigureAwait(false);
        }

        [Test]
        public async Task CreateNewApplicationReturnsResponse()
        {
            // Arrange
            var request = ConstructTestRequest();
            var json = JsonConvert.SerializeObject(request);

            // Act            
            var response = await PostTestRequestAsync(json).ConfigureAwait(false);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ApplicationResponse>(responseContent);

            // Assert            
            apiEntity.Should().NotBeNull();
            apiEntity.Id.Should().NotBeEmpty();
            apiEntity.Status.Should().Be(request.Status);
            apiEntity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 5000);
            apiEntity.Applicant.Should().BeEquivalentTo(request.Applicant);
            apiEntity.OtherMembers.Should().BeEquivalentTo(request.OtherMembers);
        }

        [Test]
        public async Task CreateNewApplicationReturnsBadRequest()
        {
            // Act
            var response = await PostTestRequestAsync("test").ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
