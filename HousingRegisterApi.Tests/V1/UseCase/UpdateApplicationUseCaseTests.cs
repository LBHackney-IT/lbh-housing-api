using AutoFixture;
using FluentAssertions;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase;
using Moq;
using NUnit.Framework;
using System;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class UpdateApplicationUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockGateway;
        private Mock<IBiddingNumberGenerator> _biddingNumberGenerator;
        private UpdateApplicationUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _biddingNumberGenerator = new Mock<IBiddingNumberGenerator>();
            _classUnderTest = new UpdateApplicationUseCase(_mockGateway.Object, _biddingNumberGenerator.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void UpdateApplicationCallsGateway()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            _mockGateway
                .Setup(x => x.UpdateApplication(id, It.IsAny<UpdateApplicationRequest>()))
                .Returns(application);

            // Act
            var response = _classUnderTest.Execute(id, new UpdateApplicationRequest());

            // Assert
            _mockGateway.Verify(x => x.UpdateApplication(id, It.IsAny<UpdateApplicationRequest>()));
            response.Should().BeEquivalentTo(application.ToResponse());
        }

        [Test]
        public void UpdateApplicationExceptionIsThrown()
        {
            // Arrange
            var id = Guid.NewGuid();
            var exception = new ApplicationException("Test exception");
            _mockGateway
                .Setup(x => x.UpdateApplication(id, It.IsAny<UpdateApplicationRequest>()))
                .Throws(exception);

            // Act
            Func<ApplicationResponse> func = () => _classUnderTest.Execute(id, new UpdateApplicationRequest());

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
