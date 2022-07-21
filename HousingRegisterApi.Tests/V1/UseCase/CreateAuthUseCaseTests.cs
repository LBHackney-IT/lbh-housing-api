using AutoFixture;
using FluentAssertions;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase;
using Moq;
using NUnit.Framework;
using System;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class CreateAuthUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockApplicationGateway;
        private Mock<INotifyGateway> _mockNotifyGateway;
        private Mock<IActivityGateway> _mockActivityGateway;
        private CreateAuthUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockApplicationGateway = new Mock<IApplicationApiGateway>();
            _mockNotifyGateway = new Mock<INotifyGateway>();
            _mockActivityGateway = new Mock<IActivityGateway>();
            _classUnderTest = new CreateAuthUseCase(_mockApplicationGateway.Object, _mockNotifyGateway.Object, _mockActivityGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void CreateVerifyCodeCallsGatewayWhenAnExistingApplicationIsIncomplete()
        {
            // Arrange
            var application = _fixture.Create<Application>();

            _mockApplicationGateway
                .Setup(x => x.GetIncompleteApplication(It.IsAny<string>()))
                .Returns(application);

            _mockApplicationGateway
                .Setup(x => x.CreateVerifyCode(application.Id, It.IsAny<CreateAuthRequest>()))
                .Returns(application);

            // Act
            var response = _classUnderTest.Execute(new CreateAuthRequest());

            // Assert
            _mockApplicationGateway.Verify(x => x.CreateVerifyCode(application.Id, It.IsAny<CreateAuthRequest>()));
            response.Should().BeOfType<CreateAuthResponse>();
        }

        [Test]
        public void CreateVerifyCodeCallsGatewayWhenNoIncompleteApplicationsExist()
        {
            // Arrange
            var application = _fixture.Create<Application>();

            _mockApplicationGateway
                .Setup(x => x.GetIncompleteApplication(It.IsAny<string>()))
                .Returns<Application>(null);

            _mockApplicationGateway
               .Setup(x => x.CreateNewApplication(It.IsAny<CreateApplicationRequest>()))
               .Returns(application);

            _mockApplicationGateway
                .Setup(x => x.CreateVerifyCode(It.IsAny<Guid>(), It.IsAny<CreateAuthRequest>()))
                .Returns(application);

            // Act
            var response = _classUnderTest.Execute(new CreateAuthRequest());

            // Assert
            _mockApplicationGateway.Verify(x => x.CreateVerifyCode(It.IsAny<Guid>(), It.IsAny<CreateAuthRequest>()));
            response.Should().BeOfType<CreateAuthResponse>();
        }
    }
}
