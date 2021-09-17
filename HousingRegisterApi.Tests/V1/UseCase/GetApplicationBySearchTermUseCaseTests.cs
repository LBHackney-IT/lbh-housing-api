using System.Linq;
using AutoFixture;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.Boundary.Request;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class GetApplicationBySearchTermUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockGateway;
        private Mock<IPaginationHelper> _mockPaginationHelper;
        private GetApplicationBySearchTermUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _mockPaginationHelper = new Mock<IPaginationHelper>();
            _classUnderTest = new GetApplicationBySearchTermUseCase(_mockGateway.Object, _mockPaginationHelper.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void GetsApplicationsFromTheGateway()
        {
            // Arrange
            var stubbedEntities = _fixture.CreateMany<Application>().ToList();
            var searchParameters = new SearchApplicationRequest();
            _mockGateway.Setup(x => x.GetAllBySearchTerm(searchParameters)).Returns(stubbedEntities);

            // Act
            var expectedResponse = _mockPaginationHelper.Object.BuildResponse(searchParameters, stubbedEntities.ToResponse(), stubbedEntities.Count);

            // Assert
            _classUnderTest.Execute(searchParameters).Should().BeEquivalentTo(expectedResponse);
        }

        //TODO: Add extra tests here for extra functionality added to the use case
    }
}
