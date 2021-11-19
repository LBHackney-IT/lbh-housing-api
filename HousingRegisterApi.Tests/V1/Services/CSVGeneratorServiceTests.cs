using AutoFixture;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.FileExport;
using HousingRegisterApi.V1.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class CSVGeneratorServiceTests
    {
        private ICsvService _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Init()
        {
            _classUnderTest = new CsvGeneratorService();
            _fixture = new Fixture();
        }

        [Test]
        public void GivenAValidEntityTheCsvGeneratorReturnsAValidByteArray()
        {
            // Arrange
            var applications = _fixture.Create<IList<NovaletExportDataRow>>().ToArray();

            // Act
            var bytes = _classUnderTest.Generate(applications);

            // Assert
            Assert.IsNotNull(bytes);
        }

        [Test]
        public void GivenAValidEntityThatContainsFieldsWithCommasTheCsvGeneratorReturnsAValidByteArray()
        {
            // Arrange
            var applications = _fixture.Create<List<NovaletExportDataRow>>();
            applications.ForEach(x => x.Address2 = "Some, Where, Here");

            // Act
            var bytes = _classUnderTest.Generate(applications.ToArray());

            // Assert
            Assert.IsNotNull(bytes);
        }

        [Test]
        public void GivenAValidEntityThatContainsFieldsWithCommasAndQuotesTheCsvGeneratorReturnsAValidByteArray()
        {
            // Arrange
            var applications = _fixture.Create<List<NovaletExportDataRow>>();
            applications.ForEach(x => x.Address2 = "Some, \"Werid,, \"Location");

            // Act
            var bytes = _classUnderTest.Generate(applications.ToArray());

            // Assert
            Assert.IsNotNull(bytes);
        }

        [Test]
        public void GivenAValidListsOfEntitiesTheCsvGeneratorReturnsAValidByteArray()
        {
            // Arrange
            var applications = _fixture.Create<List<NovaletExportDataRow>>();

            // Act
            var bytes = _classUnderTest.Generate(applications.ToArray());

            // Assert
            Assert.IsNotNull(bytes);
        }
    }
}
