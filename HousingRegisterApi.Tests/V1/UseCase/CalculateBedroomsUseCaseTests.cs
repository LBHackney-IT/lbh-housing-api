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

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class CalculateBedroomsUseCaseTests
    {
        private Mock<ILogger<CalculateBedroomsUseCase>> _loggerMock;
        private Mock<IApplicationApiGateway> _gatewayMock;
        private CalculateBedroomsUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<CalculateBedroomsUseCase>>();
            _gatewayMock = new Mock<IApplicationApiGateway>();

            _classUnderTest = new CalculateBedroomsUseCase(_loggerMock.Object, _gatewayMock.Object, new BedroomCalculatorService());
            _fixture = new Fixture();
        }

        [Test]
        public void CalculateBedroomsUseCaseLogsTheRequest()
        {
            // Arrange
            var application = _fixture.Create<Application>();
            _gatewayMock.Setup(x => x.GetApplicationById(It.IsAny<Guid>())).Returns(application);

            // Act
            var response = _classUnderTest.Execute(application.Id);

            // Assert
            _loggerMock.Verify(m => m.Log<It.IsAnyType>(LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.IsAny<It.IsAnyType>(),
                   null,
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.Once);
        }

        [Test]
        public void WhenApplicationExistThenTheCalculateBedroomsUseReturnsAValue()
        {
            // Arrange
            var application = _fixture.Create<Application>();
            _gatewayMock.Setup(x => x.GetApplicationById(It.IsAny<Guid>())).Returns(application);

            // Act
            var response = _classUnderTest.Execute(application.Id);

            // Assert
            response.Value.Should().BeGreaterOrEqualTo(0);
        }

        [Test]
        public void WhenApplicationDoesntExistThenTheCalculateBedroomsUseCaseReturnsNull()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _gatewayMock.Setup(x => x.GetApplicationById(It.IsAny<Guid>())).Returns<Application>(null);

            // Act
            SimpleTypeResponse<int?> response = _classUnderTest.Execute(id);

            // Assert
            response.Value.Should().BeNull();
        }
    }
}
