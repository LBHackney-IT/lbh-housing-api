using AutoFixture;
using FluentAssertions;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class RecalculateBedroomsUseCaseTests
    {
        private Mock<ILogger<RecalculateBedroomsUseCase>> _loggerMock;
        private Mock<IApplicationApiGateway> _gatewayMock;
        private Mock<INotifyGateway> _notifyGatewayMock;
        private RecalculateBedroomsUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<RecalculateBedroomsUseCase>>();
            _gatewayMock = new Mock<IApplicationApiGateway>();
            _notifyGatewayMock = new Mock<INotifyGateway>();

            _classUnderTest = new RecalculateBedroomsUseCase(
                _loggerMock.Object,
                _gatewayMock.Object,
                new BedroomCalculatorService(),
                _notifyGatewayMock.Object);

            _fixture = new Fixture();
        }     

        [Test]
        public void WhenRecalculatingBedroomNeedForAValidApplicationThenTheCalculationReturnsTrue()
        {
            // Arrange
            var application = _fixture.Create<Application>();
            _gatewayMock.Setup(x => x.GetApplicationsAtStatus(It.IsAny<string[]>())).Returns(new List<Application> { application });

            // Act
            bool success = _classUnderTest.Execute();

            // Assert
            success.Should().BeTrue();
        }       
    }
}
