using AutoFixture;
using FluentAssertions;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.Report.Internal;
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
    public class GetInternalReportUseCaseTests
    {
        private Mock<ILogger<GetInternalReportUseCase>> _mockLogger;
        private Mock<IApplicationApiGateway> _mockGateway;
        private Mock<IFileGateway> _mockFileGateway;
        private CsvGeneratorService _csvService;
        private GetInternalReportUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<GetInternalReportUseCase>>();
            _mockGateway = new Mock<IApplicationApiGateway>();
            _mockFileGateway = new Mock<IFileGateway>();
            _csvService = new CsvGeneratorService();
            _classUnderTest = new GetInternalReportUseCase(_mockLogger.Object, _mockGateway.Object, _mockFileGateway.Object, _csvService);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetCasesInternalReportReturnsAFile()
        {
            // Arrange
            var applications = _fixture.Create<List<Application>>();
            applications.ForEach(x => x.Status = ApplicationStatus.Active);
            var request = new InternalReportRequest
            {
                ReportType = InternalReportType.CasesReport,
                StartDate = DateTime.Now.AddDays(-7),
                EndDate = DateTime.Now.AddDays(7)
            };

            _mockGateway.Setup(x => x.GetApplicationsAtStatus(ApplicationStatus.Active)).Returns((applications));

            // Act
            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);

            // Assert
            DateTime runDate = DateTime.Now;
            response.Should().NotBeNull();
            response.FileMimeType.Should().Be("text/csv");
            response.FileName.Should().Be($"LBH-CASES REPORT-{runDate.Day}{runDate.Month}{runDate.Year}.csv");
            response.Data.Should().NotBeEmpty();
        }
    }
}
