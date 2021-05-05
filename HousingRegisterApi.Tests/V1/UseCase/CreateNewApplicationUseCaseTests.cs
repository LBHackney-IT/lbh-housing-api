using AutoFixture;
using FluentAssertions;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase;
using Moq;
using NUnit.Framework;
using System;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class CreateNewApplicationUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockGateway;
        private CreateNewApplicationUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _classUnderTest = new CreateNewApplicationUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void CreateNewApplicationCallsGateway()
        {
            // Arrange            
            var application = _fixture.Create<Application>();
            _mockGateway
                .Setup(x => x.CreateNewApplication(It.IsAny<CreateApplicationRequest>()))
                .Returns(application);

            // Act
            var response = _classUnderTest.Execute(new CreateApplicationRequest());

            // Assert
            _mockGateway.Verify(x => x.CreateNewApplication(It.IsAny<CreateApplicationRequest>()));
            response.Should().BeEquivalentTo(application.ToResponse());
        }

        [Test]
        public void CreateNewApplicationExceptionIsThrown()
        {
            // Arrange
            var exception = new ApplicationException("Test exception");
            _mockGateway
                .Setup(x => x.CreateNewApplication(It.IsAny<CreateApplicationRequest>()))
                .Throws(exception);

            // Act
            Func<ApplicationResponse> func = () => _classUnderTest.Execute(new CreateApplicationRequest());

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
