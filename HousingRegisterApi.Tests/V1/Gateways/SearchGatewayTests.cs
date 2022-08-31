using HousingRegisterApi.V1.Gateways;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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
            var outputQuery = _classUnderTest.ParseQuery(query);

            //Assert
            Assert.AreEqual(query, outputQuery.GetSimpleQueryStringWithFuzziness(), $"Expert query was changed");
        }

        [Test]
        public void FuzzinessIsApplied()
        {
            //Arrange
            string query = "Martin Hughes";

            //Act
            var outputQuery = _classUnderTest.ParseQuery(query);

            //Assert
            Assert.AreEqual(outputQuery.GetSimpleQueryStringWithFuzziness(), "Martin~1 Hughes~1", "Expected fuzziness not applied");
        }

        [Test]
        public void FuzzinessIsAppliedForLongTerms()
        {
            //Arrange
            string query = "Martin Smithson";

            //Act
            var outputQuery = _classUnderTest.ParseQuery(query);

            //Assert
            Assert.AreEqual(outputQuery.GetSimpleQueryStringWithFuzziness(), "Martin~1 Smithson~2", "Expected fuzziness not applied");
        }

        [Test]
        public void PartialNINOsAreCaptured()
        {
            //Arrange
            string query = "JL998";

            //Act
            var outputQuery = _classUnderTest.ParseQuery(query);

            //Assert
            Assert.That(outputQuery.NINOs.Count == 1);
            Assert.AreEqual(outputQuery.NINOs.First(), query, "NINO captured incorrectly");
        }

        [Test]
        public void PartialReferenceNumbersAreCaptured()
        {
            //Arrange
            string query = "bbd";

            //Act
            var outputQuery = _classUnderTest.ParseQuery(query);

            //Assert
            Assert.That(outputQuery.ReferenceNumbers.Count == 1);
            Assert.AreEqual(outputQuery.ReferenceNumbers.First(), query, "ReferenceNumber captured incorrectly");
        }

        [Test]
        public void AmbiguousTermIsInBothNinoAndReferenceList()
        {
            //Arrange
            string query = "bd6";

            //Act
            var outputQuery = _classUnderTest.ParseQuery(query);

            //Assert
            Assert.That(outputQuery.ReferenceNumbers.Count == 1);
            Assert.That(outputQuery.NINOs.Count == 1);
            Assert.AreEqual(outputQuery.ReferenceNumbers.First(), query, "Expected fuzziness not applied");
            Assert.AreEqual(outputQuery.NINOs.First(), query, "Expected fuzziness not applied");
        }

        [Test]
        public void QueryConstructionWorks()
        {
            //Arrange
            string query = "bd6";

            //Act
            var outputQuery = _classUnderTest.ConstructApplicationSearch(query, 0, 10);

            string outputQueryAsString = outputQuery.ToString();

            //Assert
            Assert.IsTrue(!string.IsNullOrWhiteSpace(outputQueryAsString));
        }
    }
}
