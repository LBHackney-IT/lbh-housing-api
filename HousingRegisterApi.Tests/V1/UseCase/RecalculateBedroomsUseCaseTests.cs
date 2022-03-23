using AutoFixture;
using FluentAssertions;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
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
        private Mock<ISnsGateway> _snsGatewayMock;
        private Mock<ISnsFactory> _snsFactoryMock;
        private Mock<IBedroomCalculatorService> _bedroomCalculatorServiceMock;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<RecalculateBedroomsUseCase>>();
            _gatewayMock = new Mock<IApplicationApiGateway>();
            _notifyGatewayMock = new Mock<INotifyGateway>();
            _snsGatewayMock = new Mock<ISnsGateway>();
            _snsFactoryMock = new Mock<ISnsFactory>();
            _bedroomCalculatorServiceMock = new Mock<IBedroomCalculatorService>();

            _classUnderTest = new RecalculateBedroomsUseCase(
                _loggerMock.Object,
                _gatewayMock.Object,
                new BedroomCalculatorService(),
                _notifyGatewayMock.Object,
                _snsGatewayMock.Object,
                _snsFactoryMock.Object);

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

        [Test]
        public void WhenRecalculatingBedroomNeedIfNewBedroomNeedIsNullSkipApplication()
        {
            // Arrange
            Application application = _fixture.Create<Application>();
            //Remove main applicant so the bedroom need calculation comes back as null
            application.MainApplicant = null;
            _bedroomCalculatorServiceMock.Setup(x => x.Calculate(application)).Returns((int?)null);

            _gatewayMock.Setup(x => x.GetApplicationsAtStatus(It.IsAny<string[]>())).Returns(new List<Application> { application });

            // Act
            bool success = _classUnderTest.Execute();

            // Assert
            _loggerMock.Verify(
               x => x.Log(
                   LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((o, t) => string.Equals($"Unable to recalculate bedroom need for application: {application.Id}", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                   It.IsAny<Exception>(),
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.AtLeastOnce);
            
            success.Should().BeTrue();
        }

        //2. else if (newBedroomNeed == currentBedroomNeed)
        [Test]
        public void IfNewBedroomNeedEqualsCurrentBedroomNeedSkipApplication()
        {
            // Arrange
            var application = _fixture.Create<Application>();
            _gatewayMock.Setup(x => x.GetApplicationsAtStatus(It.IsAny<string[]>())).Returns(new List<Application> { application });

            // Act
            bool success = _classUnderTest.Execute();

            // Assert
            success.Should().BeTrue();
        }

        //3. else if ((newBedroomNeed.HasValue && application.Assessment?.BedroomNeed.HasValue == true) && (newBedroomNeed != application.Assessment.BedroomNeed))

        //4. else if (newBedroomNeed.HasValue && currentBedroomNeed.HasValue) //This assumes they should have had a previous bedroon need for it to be recalculated

    }
}
