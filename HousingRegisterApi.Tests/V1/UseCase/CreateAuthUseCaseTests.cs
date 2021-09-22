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
    public class CreateAuthUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockApplicationGateway;
        private Mock<INotifyGateway> _mockNotifyGateway;
        private CreateAuthUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockApplicationGateway = new Mock<IApplicationApiGateway>();
            _mockNotifyGateway = new Mock<INotifyGateway>();
            _classUnderTest = new CreateAuthUseCase(_mockApplicationGateway.Object, _mockNotifyGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void CreateVerifyCodeCallsGateway()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            _mockApplicationGateway
                .Setup(x => x.CreateVerifyCode(id, It.IsAny<CreateAuthRequest>()))
                .Returns(application);

            // Act
            var response = _classUnderTest.Execute(id, new CreateAuthRequest());

            // Assert
            _mockApplicationGateway.Verify(x => x.CreateVerifyCode(id, It.IsAny<CreateAuthRequest>()));
            response.Should().BeOfType<CreateAuthResponse>();
        }
    }
}
