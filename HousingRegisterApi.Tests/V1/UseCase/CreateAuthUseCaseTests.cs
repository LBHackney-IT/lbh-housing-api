using AutoFixture;
using FluentAssertions;
using HousingRegisterApi.V1.Boundary.Request;
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
        private Mock<IApplicationApiGateway> _mockGateway;
        private CreateAuthUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _classUnderTest = new CreateAuthUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void CreateVerifyCodeCallsGateway()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            _mockGateway
                .Setup(x => x.CreateVerifyCode(id, It.IsAny<CreateAuthRequest>()))
                .Returns(application);

            // Act
            var response = _classUnderTest.Execute(id, new CreateAuthRequest());

            // Assert
            _mockGateway.Verify(x => x.CreateVerifyCode(id, It.IsAny<CreateAuthRequest>()));
            response.Should().BeEquivalentTo(application.ToResponse());
        }
    }
}
