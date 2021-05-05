using AutoFixture;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using HousingRegisterApi.V1.Boundary.Response;

namespace HousingRegisterApi.Tests.V1.E2ETests
{
    //For guidance on writing integration tests see the wiki page https://github.com/LBHackney-IT/lbh-base-api/wiki/Writing-Integration-Tests    
    public class GetApplicationByIdTest : DynamoDbIntegrationTests<Startup>
    {
        private readonly Fixture _fixture = new Fixture();

        /// <summary>
        /// Method to construct a test entity that can be used in a test
        /// </summary>        
        /// <returns></returns>
        private Application ConstructTestEntity()
        {
            var entity = _fixture.Create<Application>();
            entity.CreatedAt = DateTime.UtcNow;
            return entity;
        }

        /// <summary>
        /// Method to add an entity instance to the database so that it can be used in a test.
        /// Also adds the corresponding action to remove the upserted data from the database when the test is done.
        /// </summary>
        /// <param name="entity"></param>        
        private async Task SetupTestData(Application entity)
        {
            await DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);            
        }

        [Test]
        public async Task GetEntityByIdNotFoundReturns404()
        {
            var id = Guid.NewGuid();
            var uri = new Uri($"api/v1/applications/{id}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetEntityByIdFoundReturnsResponse()
        {
            // Arrange
            var entity = ConstructTestEntity();
            await SetupTestData(entity).ConfigureAwait(false);

            // Act
            var uri = new Uri($"api/v1/applications/{entity.Id}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<ApplicationResponse>(responseContent);

            // Assert
            apiEntity.Should().BeEquivalentTo(entity.ToResponse());
        }
    }
}
