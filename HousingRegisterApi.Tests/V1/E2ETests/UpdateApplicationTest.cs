using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
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
        /// Method to construct a test request that can be used in a test
        /// </summary>        
        /// <returns></returns>
        private UpdateApplicationRequest ConstructTestRequest()
        {
            var entity = _fixture.Create<UpdateApplicationRequest>();
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

        private async Task<HttpResponseMessage> PatchTestRequestAsync(Guid id, string input)
        {
            using var data = new StringContent(input, Encoding.UTF8, "application/json");
            var uri = new Uri($"api/v1/applications/{id}", UriKind.Relative);
            return await Client.PatchAsync(uri, data).ConfigureAwait(false);
        }

        [Test]
        public async Task UpdateApplicationReturnsResponse()
        {
            // Arrange  
            var entity = ConstructTestEntity();
            await SetupTestData(entity).ConfigureAwait(false);

            var request = ConstructTestRequest();            
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
            apiEntity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 5000);
            apiEntity.Applicant.Should().BeEquivalentTo(request.Applicant);
            apiEntity.OtherMembers.Should().BeEquivalentTo(request.OtherMembers);
        }

        [Test]
        public async Task UpdateApplicationReturnsNotFound()
        {
            // Arrange  
            var id = Guid.NewGuid();
            var request = ConstructTestRequest();
            var json = JsonConvert.SerializeObject(request);

            // Act            
            var response = await PatchTestRequestAsync(id, json).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
