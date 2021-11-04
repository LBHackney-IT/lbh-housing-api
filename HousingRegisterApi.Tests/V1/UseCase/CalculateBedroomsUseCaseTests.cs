using AutoFixture;
using FluentAssertions;
using HousingRegisterApi.V1.Boundary.Request;
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

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class CalculateBedroomsUseCaseTests
    {
        private Mock<ILogger<CalculateBedroomsUseCase>> _loggerMock;
        private CalculateBedroomsUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<CalculateBedroomsUseCase>>();
            _classUnderTest = new CalculateBedroomsUseCase(_loggerMock.Object, new BedroomCalculatorService());
            _fixture = new Fixture();
        }

        [Test]
        public void CalculateBedroomsUseCaseLogsTheRequest()
        {
            // Arrange            
            var request = _fixture.Create<CalculateBedroomsRequest>();

            // Act
            var response = _classUnderTest.Execute(request);

            // Assert
            _loggerMock.Verify(m => m.Log<It.IsAnyType>(LogLevel.Information,
                   It.IsAny<EventId>(),
                   It.IsAny<It.IsAnyType>(),
                   null,
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.Once);
        }

        [Test]
        public void WhenMainApplicantIsNullThenTheCalculateBedroomsUseCaseThrowsAnException()
        {
            // Arrange
            var request = _fixture.Create<CalculateBedroomsRequest>();
            request.MainApplicant = null;

            var exception = new ApplicationException("Applicant is missing");

            // Act
            Func<SimpleTypeResponse<int>> func = () => _classUnderTest.Execute(request);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
