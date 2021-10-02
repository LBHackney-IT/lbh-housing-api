using System.Linq;
using AutoFixture;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.Boundary.Request;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class GetAllApplicationsUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockGateway;
        private IPaginationHelper _paginationHelper;
        private GetAllApplicationsUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _paginationHelper = new PaginationHelper();
            _classUnderTest = new GetAllApplicationsUseCase(_mockGateway.Object, _paginationHelper);
            _fixture = new Fixture();
        }

        [Test]
        public void GetsAllApplicationsFromTheGateway()
        {
            // Arrange
            var queryParam = new SearchQueryParameter();
            var stubbedEntities = _fixture.CreateMany<Application>().ToList();
            _mockGateway.Setup(x => x.GetApplications(queryParam)).Returns(stubbedEntities);

            // Act
            var expectedResponse = _paginationHelper.BuildResponse(queryParam, stubbedEntities, stubbedEntities.Count);

            // Assert
            _classUnderTest.Execute(queryParam).Should().BeEquivalentTo(expectedResponse);
            Assert.AreEqual(expectedResponse.PageSize, queryParam.PageSize);
        }

        [Test]
        public void GetsAllApplicationsByAssignee()
        {
            // Arrange
            var queryParam = new SearchQueryParameter()
            {
                AssignedTo = "test@hackney.gov.uk",
                PageSize = 50
            };
            var stubbedEntities = _fixture.CreateMany<Application>().ToList();
            _mockGateway.Setup(x => x.GetApplications(queryParam)).Returns(stubbedEntities);

            // Act
            var expectedResponse = _paginationHelper.BuildResponse(queryParam, stubbedEntities, stubbedEntities.Count);

            // Assert
            _classUnderTest.Execute(queryParam).Should().BeEquivalentTo(expectedResponse);
            Assert.AreEqual(expectedResponse.PageSize, queryParam.PageSize);
        }
    }
}
