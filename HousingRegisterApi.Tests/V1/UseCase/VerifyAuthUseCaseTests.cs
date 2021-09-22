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
    public class VerifyAuthUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockGateway;
        private VerifyAuthUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _classUnderTest = new VerifyAuthUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void ConfirmVerifyCodeCallsGateway()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            _mockGateway
                .Setup(x => x.ConfirmVerifyCode(id, It.IsAny<VerifyAuthRequest>()))
                .Returns(application);

            // Act
            var response = _classUnderTest.Execute(id, new VerifyAuthRequest());

            // Assert
            _mockGateway.Verify(x => x.ConfirmVerifyCode(id, It.IsAny<VerifyAuthRequest>()));
            response.Should().BeOfType<VerifyAuthResponse>();
        }
    }
}
