using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using HousingRegisterApi.Tests.V1.Helper;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;

namespace HousingRegisterApi.Tests.V1.Gateways
{
    [TestFixture]
    public class DynamoDbGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IDynamoDBContext> _dynamoDb;
        private Mock<ISHA256Helper> _hashHelper;
        private DynamoDbGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _dynamoDb = new Mock<IDynamoDBContext>();
            _hashHelper = new Mock<ISHA256Helper>();
            _classUnderTest = new DynamoDbGateway(_dynamoDb.Object, _hashHelper.Object);
        }

        [Test]
        public void GetEntityByIdReturnsNullIfEntityDoesntExist()
        {
            // Act
            var guid = Guid.NewGuid();
            var response = _classUnderTest.GetApplicationById(guid);

            // Assert
            response.Should().BeNull();
        }

        [Test]
        public void GetEntityByIdReturnsTheEntityIfItExists()
        {
            // Arrange
            var entity = _fixture.Create<Application>();
            var dbEntity = DatabaseEntityHelper.CreateDatabaseEntityFrom(entity);

            _dynamoDb.Setup(x => x.LoadAsync<ApplicationDbEntity>(entity.Id, default))
                     .ReturnsAsync(dbEntity);

            // Act
            var response = _classUnderTest.GetApplicationById(entity.Id);

            // Assert
            _dynamoDb.Verify(x => x.LoadAsync<ApplicationDbEntity>(entity.Id, default), Times.Once);
            entity.Should().BeEquivalentTo(response);
        }
    }
}
