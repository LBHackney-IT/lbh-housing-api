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
    public class GetApplicationByIdUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockGateway;
        private Mock<IActivityHistory> _mockHistory;
        private GetApplicationByIdUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _mockHistory = new Mock<IActivityHistory>();

            _classUnderTest = new GetApplicationByIdUseCase(_mockGateway.Object, _mockHistory.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void GetApplicationByIdNullReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockGateway.Setup(x => x.GetApplicationById(id)).Returns((Application) null);

            // Act
            var response = _classUnderTest.Execute(id);

            // Assert
            response.Should().BeNull();
        }

        [Test]
        public void GetApplicationByIdFoundReturnsResponse()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            _mockGateway.Setup(x => x.GetApplicationById(id)).Returns(application);

            // Act
            var response = _classUnderTest.Execute(id);

            // Assert
            response.Should().BeEquivalentTo(application.ToResponse());
        }

        [Test]
        public void GetApplicationByIdLogsCaseViewedActivity()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            _mockGateway.Setup(x => x.GetApplicationById(id)).Returns(application);

            // Act
            var response = _classUnderTest.Execute(id);

            // Assert
            _mockHistory.Verify(x => x.LogActivity(It.IsAny<Application>(),
                It.Is<EntityActivity<ApplicationActivityType>>(x => x.ActivityType == ApplicationActivityType.CaseViewed)));
        }

        //TODO: Add extra tests here for extra functionality added to the use case
    }
}
