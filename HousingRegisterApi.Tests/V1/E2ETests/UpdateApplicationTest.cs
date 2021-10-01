using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.E2ETests
{
    //For guidance on writing integration tests see the wiki page https://github.com/LBHackney-IT/lbh-base-api/wiki/Writing-Integration-Tests
    public class UpdateApplicationTest : DynamoDbIntegrationTests<Startup>
    {
        private readonly ApplicationFixture _applicationFixture;

        public UpdateApplicationTest()
        {
            _applicationFixture = new ApplicationFixture();
        }

        private async Task SetupTestData(Application entity)
        {
            await DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> PatchTestRequestAsync(Guid id, string input)
        {
            using var data = new StringContent(input, Encoding.UTF8, "application/json");
            var uri = new Uri($"api/v1/applications/{id}", UriKind.Relative);
            return await Client.PatchAsync(uri, data).ConfigureAwait(false);
        }

        [Test]
        public async Task UpdateApplicationFullReturnsValidResponse()
        {
            // Arrange
            var entity = _applicationFixture.ConstructTestEntity();
            await SetupTestData(entity).ConfigureAwait(false);

            var request = _applicationFixture.ConstructUpdateApplicationRequest();
            var json = JsonConvert.SerializeObject(request);

            // Act
            var response = await PatchTestRequestAsync(entity.Id, json).ConfigureAwait(false);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ApplicationResponse>(responseContent);

            // Assert
            apiEntity.Should().NotBeNull();
            apiEntity.Id.Should().NotBeEmpty();
            apiEntity.Status.Should().Be(request.Status);
            apiEntity.SensitiveData.Should().Be(request.SensitiveData.Value);
            apiEntity.AssignedTo.Should().Be(request.AssignedTo);
            apiEntity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 5000);
            apiEntity.MainApplicant.Should().BeEquivalentTo(request.MainApplicant);
            apiEntity.OtherMembers.Should().BeEquivalentTo(request.OtherMembers);
            apiEntity.Assessment.Should().BeEquivalentTo(request.Assessment);
        }

        [Test]
        public async Task UpdateApplicationPartialReturnsValidResponse()
        {
            // Arrange
            var entity = _applicationFixture.ConstructTestEntity();
            await SetupTestData(entity).ConfigureAwait(false);

            var request = new UpdateApplicationRequest()
            {
                Status = "Pending",
                SensitiveData = false,
                AssignedTo = "test@hackney.gov.uk",
                OtherMembers = new List<Applicant>()
            };
            var json = JsonConvert.SerializeObject(request);

            // Act
            var response = await PatchTestRequestAsync(entity.Id, json).ConfigureAwait(false);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ApplicationResponse>(responseContent);

            // Assert
            apiEntity.Should().NotBeNull();
            apiEntity.Id.Should().NotBeEmpty();
            apiEntity.Status.Should().Be(request.Status);
            apiEntity.SensitiveData.Should().Be(request.SensitiveData.Value);
            apiEntity.AssignedTo.Should().Be(request.AssignedTo);
            apiEntity.CreatedAt.Should().Be(entity.CreatedAt);
            apiEntity.MainApplicant.Should().BeEquivalentTo(entity.MainApplicant);
            apiEntity.OtherMembers.Should().BeEquivalentTo(request.OtherMembers);
        }

        [Test]
        public async Task UpdateApplicationReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = _applicationFixture.ConstructUpdateApplicationRequest();
            var json = JsonConvert.SerializeObject(request);

            // Act
            var response = await PatchTestRequestAsync(id, json).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
