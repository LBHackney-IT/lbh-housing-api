using System.Linq;
using AutoFixture;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class GetAllApplicationsUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockGateway;
        private GetAllApplicationsUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _classUnderTest = new GetAllApplicationsUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void GetsAllApplicationsFromTheGateway()
        {
            // Arrange
            var stubbedEntities = _fixture.CreateMany<Application>().ToList();
            _mockGateway.Setup(x => x.GetAll()).Returns(stubbedEntities);

            // Act
            var expectedResponse = new ApplicationList { Results = stubbedEntities.ToResponse() };

            // Assert
            _classUnderTest.Execute().Should().BeEquivalentTo(expectedResponse);
        }

        //TODO: Add extra tests here for extra functionality added to the use case
    }
}
