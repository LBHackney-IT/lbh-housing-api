using HousingRegisterApi.V1.Gateways;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace HousingRegisterApi.Tests.V1.Gateways
{
    [TestFixture]
    public class SearchGatewayTests
    {
        private SearchGateway _classUnderTest;
        private Mock<ILogger<SearchGateway>> _loggerMock;
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<SearchGateway>>();

            //Arrange
            var inMemorySettings = new Dictionary<string, string>
            {
                {"SEARCHDOMAIN", "http://localhost:9200"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _classUnderTest = new SearchGateway(_loggerMock.Object, _configuration);
        }

        [Test]
        public void ExpertQueriesAreUntouched()
        {
            //Arrange
            string query = "Martin Hughes~2";

            //Act
            string outputQuery = SearchGateway.ProcessFuzzyMatching(query);

            //Assert
            Assert.AreEqual(query, outputQuery, $"Expert query was changed");
        }

        [Test]
        public void FuzzinessIsApplied()
        {
            //Arrange
            string query = "Martin Hughes";

            //Act
            string outputQuery = SearchGateway.ProcessFuzzyMatching(query);

            //Assert
            Assert.AreEqual(outputQuery, "Martin~1 Hughes~1", "Expected fuzziness not applied");
        }
    }
}
