using AutoFixture;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.FileExport;
using HousingRegisterApi.V1.Services;
using NUnit.Framework;
using System.Collections.Generic;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class CSVGeneratorServiceTests
    {
        private ICSVService _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Init()
        {
            _classUnderTest = new CSVGeneratorService();
            _fixture = new Fixture();
        }

        [Test]
        public void GivenAValidEntityTheCsvGeneratorReturnsAValidByteArray()
        {
            // Arrange
            var application = _fixture.Create<Application>();

            // Act
            var bytes = _classUnderTest.Generate(application, new CsvParameters
            {
                IncludeHeaders = true
            }); ;

            // Assert
            Assert.IsNotNull(bytes);
        }

        [Test]
        public void GivenAValidListsOfEntitiesTheCsvGeneratorReturnsAValidByteArray()
        {
            // Arrange
            var applications = _fixture.Create<List<Application>>();

            // Act
            var bytes = _classUnderTest.Generate(applications, new CsvParameters
            {
                IncludeHeaders = true
            }); ;

            // Assert
            Assert.IsNotNull(bytes);
        }
    }
}
