using FluentAssertions;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
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
    public class AddApplicationNoteTest : DynamoDbIntegrationTests<Startup>
    {
        private readonly ApplicationFixture _applicationFixture;

        public AddApplicationNoteTest()
        {
            _applicationFixture = new ApplicationFixture();
        }

        private async Task<HttpResponseMessage> PostTestRequestAsync(Guid id, string request)
        {
            using var data = new StringContent(request, Encoding.UTF8, "application/json");
            var uri = new Uri($"api/v1/applications/{id}/note", UriKind.Relative);
            return await Client.PostAsync(uri, data).ConfigureAwait(false);
        }

        private async Task SetupTestData(Application entity)
        {
            await DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
        }

        [Test]
        [Ignore("Ignore for S3")]
        public async Task AddingANoteToAnApplicationReturnsOk()
        {
            // Arrange
            var entity = _applicationFixture.ConstructTestEntity();
            entity.Status = ApplicationStatus.Active;
            await SetupTestData(entity).ConfigureAwait(false);

            // Act
            var request = new AddApplicationNoteRequest { Note = "Just adding some extra data" };
            var stringRequest = JsonConvert.SerializeObject(request);

            var response = await PostTestRequestAsync(entity.Id, stringRequest).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
