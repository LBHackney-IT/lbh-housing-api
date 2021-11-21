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
    public class ApproveNovaletExportTest : DynamoDbIntegrationTests<Startup>
    {
        private async Task<HttpResponseMessage> PostTestRequestAsync(string requestBody)
        {
            using var data = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var uri = new Uri($"api/v1/reporting/approvenovaletexport", UriKind.Relative);
            return await Client.PostAsync(uri, data).ConfigureAwait(false);
        }

        [Test]
        public async Task ApproveNovaletForExportSetsTheFileStatusToApproved()
        {
            // Arrange
            ApproveExportFileRequest request = new ApproveExportFileRequest { FileName = "samplefile.txt" };
            var json = JsonConvert.SerializeObject(request);
            await CreateTestFile(request.FileName).ConfigureAwait(false);

            // Act            
            var response = await PostTestRequestAsync(json).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var fileTags = await GetFileTags(request.FileName).ConfigureAwait(false);
            Assert.IsTrue(fileTags.Exists(x => x.Key == "ApprovedForExport"));
            Assert.IsTrue(fileTags.Find(x => x.Key == "ApprovedForExport")?.Value == "true");
        }
    }
}
