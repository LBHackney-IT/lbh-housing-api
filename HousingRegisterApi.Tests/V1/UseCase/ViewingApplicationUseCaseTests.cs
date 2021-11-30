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
    public class ViewingApplicationUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockGateway;
        private Mock<IActivityGateway> _mockActivityGateway;
        private ViewingApplicationUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _mockActivityGateway = new Mock<IActivityGateway>();

            _classUnderTest = new ViewingApplicationUseCase(_mockGateway.Object, _mockActivityGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void ViewingApplicationUseCaseLogsCaseViewedByUserActivity()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            _mockGateway.Setup(x => x.GetApplicationById(id)).Returns(application);

            // Act
            var response = _classUnderTest.Execute(id);

            // Assert
            _mockActivityGateway.Verify(x => x.LogActivity(It.IsAny<Application>(),
                It.Is<EntityActivity<ApplicationActivityType>>(x => x.ActivityType == ApplicationActivityType.CaseViewedByUser)));
        }
    }
}
