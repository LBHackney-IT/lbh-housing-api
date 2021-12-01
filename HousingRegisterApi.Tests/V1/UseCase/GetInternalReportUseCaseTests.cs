using AutoFixture;
using FluentAssertions;
using Hackney.Shared.ActivityHistory.Boundary.Response;
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
        private Mock<IActivityGateway> _mockActivityGateway;
        private CsvGeneratorService _csvService;
        private GetInternalReportUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<GetInternalReportUseCase>>();
            _mockGateway = new Mock<IApplicationApiGateway>();
            _mockFileGateway = new Mock<IFileGateway>();
            _mockActivityGateway = new Mock<IActivityGateway>();

            _csvService = new CsvGeneratorService();
            _classUnderTest = new GetInternalReportUseCase(_mockLogger.Object,
                _mockGateway.Object, _mockFileGateway.Object, _mockActivityGateway.Object, _csvService);

            _fixture = new Fixture();
        }

        [Test]
        public async Task GetCasesInternalReportReturnsAFile()
        {
            // Arrange
            SetupDataAndRequest(InternalReportType.CasesReport, out InternalReportRequest request);

            // Act
            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);

            // Assert
            DateTime runDate = DateTime.Now;
            response.Should().NotBeNull();
            response.FileMimeType.Should().Be("text/csv");
            response.FileName.Should().Be($"LBH-CASES REPORT-{runDate:ddMMyyyy}.csv");
            response.Data.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetPeopleInternalReportReturnsAFile()
        {
            // Arrange
            SetupDataAndRequest(InternalReportType.PeopleReport, out InternalReportRequest request);

            // Act
            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);

            // Assert
            DateTime runDate = DateTime.Now;
            response.Should().NotBeNull();
            response.FileMimeType.Should().Be("text/csv");
            response.FileName.Should().Be($"LBH-PEOPLE REPORT-{runDate:ddMMyyyy}.csv");
            response.Data.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetCaseActivityInternalReportReturnsAFile()
        {
            // Arrange
            SetupDataAndRequest(InternalReportType.CaseActivityReport, out InternalReportRequest request);

            // Act
            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);

            // Assert
            DateTime runDate = DateTime.Now;
            response.Should().NotBeNull();
            response.FileMimeType.Should().Be("text/csv");
            response.FileName.Should().Be($"LBH-CASE-ACTIVITY REPORT-{runDate:ddMMyyyy}.csv");
            response.Data.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetOfficerActivityInternalReportReturnsAFile()
        {
            // Arrange
            SetupDataAndRequest(InternalReportType.OfficerActivityReport, out InternalReportRequest request);

            // Act
            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);

            // Assert
            DateTime runDate = DateTime.Now;
            response.Should().NotBeNull();
            response.FileMimeType.Should().Be("text/csv");
            response.FileName.Should().Be($"LBH-OFFICER-ACTIVITY REPORT-{runDate:ddMMyyyy}.csv");
            response.Data.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetCaseActivityInternalReportWithNullOldAndNewDataReturnsAFile()
        {
            // Arrange
            SetupDataAndRequest(InternalReportType.CaseActivityReport, out InternalReportRequest request, true);

            // Act
            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);

            // Assert
            response.Should().NotBeNull();
            response.FileMimeType.Should().Be("text/csv");
            response.Data.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetOfficerActivityInternalReportWithNullOldAndNewDataReturnsAFile()
        {
            // Arrange
            SetupDataAndRequest(InternalReportType.OfficerActivityReport, out InternalReportRequest request, true);

            // Act
            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);

            // Assert
            response.Should().NotBeNull();
            response.FileMimeType.Should().Be("text/csv");
            response.Data.Should().NotBeEmpty();
        }

        private void SetupDataAndRequest(InternalReportType reportType, out InternalReportRequest request, bool nullifyData = false)
        {
            var applications = _fixture.Create<List<Application>>();
            applications.ForEach(x =>
            {
                x.CreatedAt = DateTime.UtcNow;
            });

            IList<ActivityHistoryResponseObject> a = new List<ActivityHistoryResponseObject>();

            var activities = _fixture.Create<List<ActivityHistoryResponseObject>>();
            activities.ForEach(x =>
            {
                if (nullifyData)
                {
                    x.NewData = null;
                    x.OldData = null;
                }
                else
                {
                    x.NewData.Add("_activityType", "CaseViewedByUser");
                }
            });

            _mockGateway.Setup(x => x.GetApplications(It.IsAny<SearchQueryParameter>())).Returns(applications);
            _mockActivityGateway.Setup(x => x.GetActivities(It.IsAny<Guid>())).Returns(Task.FromResult(activities));

            request = new InternalReportRequest
            {
                ReportType = reportType,
                StartDate = DateTime.Now.AddDays(-7),
                EndDate = DateTime.Now.AddDays(7)
            };
        }
    }
}
