using FluentAssertions;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HousingRegisterApi.Tests.V1.E2ETests
{
    //For guidance on writing integration tests see the wiki page https://github.com/LBHackney-IT/lbh-base-api/wiki/Writing-Integration-Tests    
    public class ApproveNovaletExportTest : DynamoDbIntegrationTests<Startup>
    {
        private async Task<HttpResponseMessage> PostTestRequestAsync(string fileName)
        {
            var uri = new Uri($"api/v1/reporting/approvenovaletexport/" + fileName, UriKind.Relative);
            return await Client.PostAsync(uri, null).ConfigureAwait(false);
        }

        [Test]
        [Ignore("Ignore for S3")]
        public async Task ApprovingANovaletExportSetsTheStatusOnTheFileToApproved()
        {
            // Arrange
            string fileName = "samplefile.csv";
            string fileKey = "NOVALET/" + fileName;
            await CreateTestFile(fileKey).ConfigureAwait(false);
            var tags = await GetFileTags(fileKey).ConfigureAwait(false);
            var status = tags.Find(x => x.Key == "approvedOn")?.Value ?? null;
            Assert.IsNull(status);

            // Act            
            var response = await PostTestRequestAsync(fileName).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            tags = await GetFileTags(fileKey).ConfigureAwait(false);
            status = tags.Find(x => x.Key == "approvedOn")?.Value ?? null;
            Assert.IsNotNull(status);
        }
    }
}
