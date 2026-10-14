using System;
using System.Collections.Generic;
using System.Threading;
using HousingRegisterApi.V1.UseCase;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    [TestFixture]
    public class DbHealthCheckUseCaseTests
    {

        private Mock<HealthCheckService> _mockHealthCheckService;
        private DbHealthCheckUseCase _classUnderTest;

        private readonly Faker _faker = new Faker();
        private string _description;

        [SetUp]
        public void SetUp()
        {
            _description = _faker.Random.Words();

            _mockHealthCheckService = new Mock<HealthCheckService>();
            var report = new HealthReport(
                new Dictionary<string, HealthReportEntry>
                {
                    ["test"] = new HealthReportEntry(
                        HealthStatus.Healthy,
                        _description,
                        TimeSpan.Zero,
                        exception: null,
                        data: null)
                },
                TimeSpan.Zero);

            _mockHealthCheckService.Setup(s =>
                    s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(report);

            _classUnderTest = new DbHealthCheckUseCase(_mockHealthCheckService.Object);
        }

        [Test]
        public void ReturnsResponseWithStatus()
        {
            var response = _classUnderTest.Execute();

            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Message.Should().BeEquivalentTo("test: " + _description);
        }
    }
}
