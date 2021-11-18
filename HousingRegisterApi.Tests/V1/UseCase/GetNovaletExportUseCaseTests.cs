using AutoFixture;
using FluentAssertions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class GetNovaletExportUseCaseTests
    {
        private Mock<ILogger<GetNovaletExportUseCase>> _loggerMock;
        private Mock<IApplicationApiGateway> _mockGateway;
        private CSVGeneratorService _csvService;
        private GetNovaletExportUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<GetNovaletExportUseCase>>();
            _mockGateway = new Mock<IApplicationApiGateway>();
            _csvService = new CSVGeneratorService();
            _classUnderTest = new GetNovaletExportUseCase(_loggerMock.Object, _mockGateway.Object, _csvService);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetNovaletExportForAValidApplicationReturnsAFile()
        {
            // Arrange
            var application = _fixture.Create<Application>();
            _mockGateway.Setup(x => x.GetApplicationById(application.Id)).Returns((application));

            // Act
            var response = await _classUnderTest.Execute(application.Id).ConfigureAwait(false);

            // Assert
            DateTime runDate = DateTime.Now;
            response.Should().NotBeNull();
            response.FileMimeType.Should().Be("text/csv");
            response.FileName.Should().Be($"LBH-APPLICANT FEED-{runDate.Year}{runDate.Month}{runDate.Day}");
            response.Data.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetNovaletExportForAValidApplicationReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockGateway.Setup(x => x.GetApplicationById(It.IsAny<Guid>())).Returns<Application>(null);

            // Act
            var response = await _classUnderTest.Execute(id).ConfigureAwait(false);

            // Assert
            response.Should().BeNull();
        }
    }
}
