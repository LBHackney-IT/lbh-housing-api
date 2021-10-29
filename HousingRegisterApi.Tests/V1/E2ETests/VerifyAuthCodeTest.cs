using FluentAssertions;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
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
    public class VerifyAuthCodeTest : DynamoDbIntegrationTests<Startup>
    {
        private readonly AuthFixture _authFixture;
        private readonly ApplicationFixture _applicationFixture;
        public VerifyAuthCodeTest()
        {
            _authFixture = new AuthFixture();
            _applicationFixture = new ApplicationFixture();
        }

        private async Task SetupTestData(Application entity)
        {
            await DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> PostTestRequestAsync(Guid applicationId, string requestBody)
        {
            using var data = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var uri = new Uri($"api/v1/auth/{applicationId.ToString()}/verify", UriKind.Relative);
            return await Client.PostAsync(uri, data).ConfigureAwait(false);
        }

        [Test]
        public async Task InvalidVerificationAuthCodeAndApplicationIdReturnsNotFound()
        {
            // Arrange
            Guid applicationId = Guid.NewGuid();
            var request = _authFixture.ConstructVerifyAuthRequestRequest();
            var json = JsonConvert.SerializeObject(request);

            // Act
            var response = await PostTestRequestAsync(applicationId, json).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task ExpiredVerificationAuthCodeForApplicationReturnsNotFound()
        {
            // Arrange
            var application = _applicationFixture.ConstructTestEntity();
            application.Status = "New";
            application.VerifyExpiresAt = DateTime.UtcNow.AddMinutes(-25);
            await SetupTestData(application).ConfigureAwait(false);

            var request = _authFixture.ConstructVerifyAuthRequestRequest();
            request.Email = application.MainApplicant.ContactInformation.EmailAddress;
            request.Code = application.VerifyCode;
            var json = JsonConvert.SerializeObject(request);

            // Act
            var response = await PostTestRequestAsync(application.Id, json).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task ValidVerificationAuthCodeAndApplicationIdReturnsOk()
        {
            // Arrange
            var application = _applicationFixture.ConstructTestEntity();
            application.Status = "New";
            application.VerifyExpiresAt = DateTime.UtcNow.AddMinutes(25);

            await SetupTestData(application).ConfigureAwait(false);

            var request = _authFixture.ConstructVerifyAuthRequestRequest();
            request.Email = application.MainApplicant.ContactInformation.EmailAddress;
            request.Code = application.VerifyCode;

            var json = JsonConvert.SerializeObject(request);

            // Act
            var response = await PostTestRequestAsync(application.Id, json).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
