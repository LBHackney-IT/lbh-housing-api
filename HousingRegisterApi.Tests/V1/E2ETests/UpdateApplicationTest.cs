using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.Sns;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Infrastructure;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.E2ETests
{
    //For guidance on writing integration tests see the wiki page https://github.com/LBHackney-IT/lbh-base-api/wiki/Writing-Integration-Tests
    public class UpdateApplicationTest : DynamoDbIntegrationTests<Startup>
    {
        private readonly ApplicationFixture _applicationFixture;
        private const string Token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";

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
            //using var data = new StringContent(input, Encoding.UTF8, "application/json");
            //var uri = new Uri($"api/v1/applications/{id}", UriKind.Relative);
            //return await Client.PatchAsync(uri, data).ConfigureAwait(false);

            var uri = new Uri($"api/v1/applications/{id}", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Headers.Add("Authorization", Token);

            message.Content = new StringContent(input, Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Patch;

            Client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await Client.SendAsync(message).ConfigureAwait(false);
            message.Dispose();

            return response;
        }

        private static void VerifySnsMessage(ApplicationSns message, ApplicationResponse responseObject)
        {
            message.CorrelationId.Should().NotBeEmpty();
            message.DateTime.Should().BeCloseTo(DateTime.UtcNow, 5000);
            message.EntityId.Should().Be(responseObject.Id);

            message.EventData.OldData.Should().BeNull();
            // TODO - Verify that actual contents of the new data?
            message.EventData.NewData.Should().NotBeNull();

            message.EventType.Should().Be(UpdateApplicationConstants.EVENTTYPE);
            message.Id.Should().NotBeEmpty();
            message.SourceDomain.Should().Be(UpdateApplicationConstants.SOURCEDOMAIN);
            message.SourceSystem.Should().Be(UpdateApplicationConstants.SOURCESYSTEM);
            message.User.Email.Should().Be("e2e-testing@development.com");
            message.User.Name.Should().Be("Tester");
            message.Version.Should().Be(UpdateApplicationConstants.V1VERSION);
        }

        [Test]
        [Ignore("Ignore for S3")]
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
            apiEntity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 10000);
            apiEntity.MainApplicant.Should().BeEquivalentTo(request.MainApplicant);
            apiEntity.OtherMembers.Should().BeEquivalentTo(request.OtherMembers);
            // apiEntity.Assessment.Should().BeEquivalentTo(request.Assessment);
            SnsVerifer.VerifySnsEventRaised((actual) => VerifySnsMessage(actual, apiEntity));
        }

        [Test]
        [Ignore("Ignore for S3")]
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

            SnsVerifer.VerifySnsEventRaised((actual) => VerifySnsMessage(actual, apiEntity));
        }

        [Test]
        [Ignore("Ignore for S3")]
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
