using AutoFixture;
using FluentAssertions;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase;
using Moq;
using NUnit.Framework;
using System;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class ImportApplicationUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockGateway;
        private Mock<IActivityGateway> _mockActivityGateway;
        private ImportApplicationUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _mockActivityGateway = new Mock<IActivityGateway>();

            _classUnderTest = new ImportApplicationUseCase(_mockGateway.Object, _mockActivityGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void ImportApplicationCallsGateway()
        {
            // Arrange
            var application = _fixture.Create<Application>();
            _mockGateway
                .Setup(x => x.ImportApplication(It.IsAny<ImportApplicationRequest>()))
                .Returns(application);

            // Act
            var response = _classUnderTest.Execute(new ImportApplicationRequest());

            // Assert
            _mockGateway.Verify(x => x.ImportApplication(It.IsAny<ImportApplicationRequest>()));
            response.Should().BeEquivalentTo(application.ToResponse());
        }

        [Test]
        public void ImportApplicationExceptionIsThrown()
        {
            // Arrange
            var exception = new ApplicationException("Test exception");
            _mockGateway
                .Setup(x => x.ImportApplication(It.IsAny<ImportApplicationRequest>()))
                .Throws(exception);

            // Act
            Func<ApplicationResponse> func = () => _classUnderTest.Execute(new ImportApplicationRequest());

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
