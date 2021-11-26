using FluentAssertions;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
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
        private async Task<HttpResponseMessage> PostTestRequestAsync(string fileName)
        {
            var uri = new Uri($"api/v1/reporting/approvenovaletexport/" + fileName, UriKind.Relative);
            return await Client.PostAsync(uri, null).ConfigureAwait(false);
        }

        [Test]
        public async Task ApprovingANovaletExportSetsTheStatusOnTheFileToApproved()
        {
            // Arrange
            string fileName = "samplefile.csv";
            await CreateTestFile(fileName).ConfigureAwait(false);
            var tags = await GetFileTags(fileName).ConfigureAwait(false);
            Assert.IsFalse(tags.Exists(x => x.Key.Equals("status")));

            // Act            
            var response = await PostTestRequestAsync(fileName).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            tags = await GetFileTags(fileName).ConfigureAwait(false);
            Assert.IsTrue(tags.Find(x => x.Key.Equals("status")).Value == "approved");
        }
    }
}
