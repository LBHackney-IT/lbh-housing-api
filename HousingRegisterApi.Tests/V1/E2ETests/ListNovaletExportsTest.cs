using FluentAssertions;
using HousingRegisterApi.V1.Domain.Report;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HousingRegisterApi.Tests.V1.E2ETests
{
    //For guidance on writing integration tests see the wiki page https://github.com/LBHackney-IT/lbh-base-api/wiki/Writing-Integration-Tests    
    public class ListNovaletExportsTest : DynamoDbIntegrationTests<Startup>
    {
        private async Task<HttpResponseMessage> GetTestRequestAsync()
        {
            var uri = new Uri($"api/v1/reporting/listnovaletfiles", UriKind.Relative);
            return await Client.GetAsync(uri).ConfigureAwait(false);
        }

        [Test]
        
        public async Task ListNovaletExportsReturnsResponse()
        {
            // Arrange
            await CreateTestFile("NOVALET/samplefile.csv").ConfigureAwait(false);

            // Act            
            var response = await GetTestRequestAsync().ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        
        public async Task ListNovaletExportsReturnsAListOfFiles()
        {
            // Arrange
            await CreateTestFile("NOVALET/samplefile.csv").ConfigureAwait(false);

            // Act            
            var response = await GetTestRequestAsync().ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var responseValue = JsonConvert.DeserializeObject<List<ExportFileItem>>(responseContent);

            responseValue.Count.Should().BeGreaterOrEqualTo(1);
        }
    }
}
