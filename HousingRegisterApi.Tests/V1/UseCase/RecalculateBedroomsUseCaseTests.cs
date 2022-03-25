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
using System.Linq;

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
                _bedroomCalculatorServiceMock.Object,
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

            _gatewayMock.Setup(x => x.GetApplicationsAtStatus(0, 0, It.IsAny<string[]>())).Returns(new List<Application> { application });

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

            _gatewayMock.Setup(x => x.GetApplicationsAtStatus(0, 0, It.IsAny<string[]>())).Returns(new List<Application> { application });

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

        [Test]
        public void IfNewBedroomNeedEqualsCurrentBedroomNeedSkipApplication()
        {
            // Arrange
            //Set both current and new to the same value
            int currentBedroomNeed = 1;
            int newBedroomNeed = 1;
            Application application = _fixture.Create<Application>();
            application.CalculatedBedroomNeed = currentBedroomNeed;
            application.Assessment.BedroomNeed = null;

            _bedroomCalculatorServiceMock.Setup(x => x.Calculate(application)).Returns(newBedroomNeed);
            _gatewayMock.Setup(x => x.GetApplicationsAtStatus(0, 0, It.IsAny<string[]>())).Returns(new List<Application> { application });

            // Act
            bool success = _classUnderTest.Execute();

            // Assert
            _loggerMock.Verify(
               x => x.Log(
                   LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((o, t) => string.Equals($"No bedroom changes for application: {application.Id}", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                   It.IsAny<Exception>(),
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.AtLeastOnce);

            success.Should().BeTrue();
        }

        [Test]
        public void CheckIfApplicationNeedsReassessment()
        {
            // Arrange
            int currentBedroomNeed = 1;
            int newBedroomNeed = 2;
            Application application = _fixture.Create<Application>();
            application.Assessment.BedroomNeed = currentBedroomNeed;
            application.MainApplicant.Person.Gender = "Male";
            application.MainApplicant.Person.RelationshipType = "Applicant";
            application.MainApplicant.Person.DateOfBirth = DateTime.Now.AddYears(-22);

            //Set new bedroom need to be different
            _bedroomCalculatorServiceMock.Setup(x => x.Calculate(application)).Returns(newBedroomNeed);
            _gatewayMock.Setup(x => x.GetApplicationsAtStatus(0, 0, It.IsAny<string[]>())).Returns(new List<Application> { application });

            // Act
            bool success = _classUnderTest.Execute();

            // Assert
            _loggerMock.Verify(
               x => x.Log(
                   LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((o, t) => string.Equals($"Application status for {application.Id} set to Awaiting Reassessment as bedroom need was manually set to {application.Assessment.BedroomNeed}", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                   It.IsAny<Exception>(),
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.AtLeastOnce);

            success.Should().BeTrue();
        }

        [Test]
        public void CheckIfRecalculationOccurs()
        {
            // Arrange
            int currentBedroomNeed = 1;
            int newBedroomNeed = 2;
            Application application = _fixture.Create<Application>();
            application.CalculatedBedroomNeed = currentBedroomNeed;
            application.Assessment.BedroomNeed = null;

            //Set new bedroom need to be different
            _bedroomCalculatorServiceMock.Setup(x => x.Calculate(application)).Returns(newBedroomNeed);
            _gatewayMock.Setup(x => x.GetApplicationsAtStatus(0, 0, It.IsAny<string[]>())).Returns(new List<Application> { application });

            // Act
            bool success = _classUnderTest.Execute();

            // Assert
            _loggerMock.Verify(
               x => x.Log(
                   LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((o, t) => string.Equals($"Bedroom need for application {application.Id} recalculated from '{currentBedroomNeed}' to '{newBedroomNeed}'", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                   It.IsAny<Exception>(),
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.AtLeastOnce);

            success.Should().BeTrue();
        }

        [Test]
        public void IfCurrentBandIsNullSkipApplication()
        {
            // Arrange
            Application application = _fixture.Create<Application>();
            application.CalculatedBedroomNeed = 1;
            application.Assessment.BedroomNeed = null;
            application.Assessment.Band = null;

            _bedroomCalculatorServiceMock.Setup(x => x.Calculate(application)).Returns(application.CalculatedBedroomNeed);
            _gatewayMock.Setup(x => x.GetApplicationsAtStatus(0, 0, It.IsAny<string[]>())).Returns(new List<Application> { application });

            // Act
            bool success = _classUnderTest.Execute();

            // Assert
            _loggerMock.Verify(
               x => x.Log(
                   LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((o, t) => string.Equals($"No band found for application: {application.Id}", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                   It.IsAny<Exception>(),
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.AtLeastOnce);

            success.Should().BeTrue();
        }

    }
}
