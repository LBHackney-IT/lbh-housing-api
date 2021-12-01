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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class CreateNovaletExportUseCaseTests
    {
        private Mock<ILogger<CreateNovaletExportUseCase>> _mockLogger;
        private Mock<IApplicationApiGateway> _mockGateway;
        private Mock<IFileGateway> _mockFileGateway;
        private CsvGeneratorService _csvService;
        private CreateNovaletExportUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<CreateNovaletExportUseCase>>();
            _mockGateway = new Mock<IApplicationApiGateway>();
            _mockFileGateway = new Mock<IFileGateway>();
            _csvService = new CsvGeneratorService();
            _classUnderTest = new CreateNovaletExportUseCase(_mockLogger.Object, _mockGateway.Object, _mockFileGateway.Object, _csvService);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetNovaletExportForAValidApplicationReturnsAFile()
        {
            // Arrange
            var applications = _fixture.Create<List<Application>>();
            applications.ForEach(x => x.Status = ApplicationStatus.Active);

            _mockGateway.Setup(x => x.GetApplicationsAtStatus(ApplicationStatus.Active)).Returns((applications));

            // Act
            var response = await _classUnderTest.Execute().ConfigureAwait(false);

            // Assert
            DateTime runDate = DateTime.Now;
            response.Should().NotBeNull();
            response.FileMimeType.Should().Be("text/csv");
            response.FileName.Should().Be($"LBH-APPLICANT FEED-{runDate:ddMMyyyy}.csv");
            response.Data.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetNovaletExportForAValidApplicationReturnsNull()
        {
            // Arrange
            _mockGateway.Setup(x => x.GetApplicationsAtStatus(It.IsAny<string>())).Returns(new List<Application>());

            // Act
            var response = await _classUnderTest.Execute().ConfigureAwait(false);

            // Assert
            response.Should().BeNull();
        }
    }
}
