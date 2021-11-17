using AutoFixture;
using FluentAssertions;
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
    public class CompleteApplicationUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockGateway;
        private Mock<IActivityHistory> _mockHistory;
        private CompleteApplicationUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _mockHistory = new Mock<IActivityHistory>();

            _classUnderTest = new CompleteApplicationUseCase(_mockGateway.Object, _mockHistory.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void CompleteApplicationUseCaseLogsCaseSubmittedActivity()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            _mockGateway.Setup(x => x.CompleteApplication(id)).Returns(application);

            // Act
            var response = _classUnderTest.Execute(id);

            // Assert
            _mockHistory.Verify(x => x.LogActivity(It.IsAny<Application>(),
                It.Is<EntityActivity<ApplicationActivityType>>(x => x.ActivityType == ApplicationActivityType.SubmittedByResident)));
        }

        //TODO: Add extra tests here for extra functionality added to the use case
    }
}
