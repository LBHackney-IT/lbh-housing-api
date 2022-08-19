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
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.Boundary.Request;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Logging;

namespace HousingRegisterApi.Tests.V1.Gateways
{
    [TestFixture]
    public class DynamoDbGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IDynamoDBContext> _dynamoDbContext;
        private Mock<IAmazonDynamoDB> _dynamoDb;
        private ISHA256Helper _hashHelper;
        private Mock<IVerifyCodeGenerator> _codeGenerator;
        private DynamoDbGateway _classUnderTest;
        private Mock<IBedroomCalculatorService> _bedroomCalculatorService;
        private Mock<ILogger<DynamoDbGateway>> _logger;

        [SetUp]
        public void Setup()
        {
            _dynamoDbContext = new Mock<IDynamoDBContext>();
            _hashHelper = new SHA256Helper();
            _dynamoDb = new Mock<IAmazonDynamoDB>();
            _codeGenerator = new Mock<IVerifyCodeGenerator>();
            _bedroomCalculatorService = new Mock<IBedroomCalculatorService>();
            _logger = new Mock<ILogger<DynamoDbGateway>>();
            _classUnderTest = new DynamoDbGateway(_dynamoDbContext.Object, _dynamoDb.Object, _hashHelper, _codeGenerator.Object, _bedroomCalculatorService.Object, _logger.Object);
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

            _dynamoDbContext.Setup(x => x.LoadAsync<ApplicationDbEntity>(entity.Id, default))
                     .ReturnsAsync(dbEntity);

            // Act
            var response = _classUnderTest.GetApplicationById(entity.Id);

            // Assert
            _dynamoDbContext.Verify(x => x.LoadAsync<ApplicationDbEntity>(entity.Id, default), Times.Once);
            entity.Should().BeEquivalentTo(response);
        }

        [Test]
        public void CreateNewEntityHasCorrectHashWhenEmailIsWhitespace()
        {
            // Arrange
            var createRequest = _fixture.Create<CreateApplicationRequest>();
            createRequest.MainApplicant.ContactInformation.EmailAddress = " ";

            // Act
            var application = _classUnderTest.CreateNewApplication(createRequest);

            // Assert
            Assert.AreEqual(application.Reference, _hashHelper.Generate(application.Id.ToString()).Substring(0, 10), "Reference is incorrect");
        }

        [Test]
        public void CreateNewEntityHasCorrectHashWhenEmailIsPresent()
        {
            // Arrange
            string emailAddress = "test.email@testdomain.com";
            var createRequest = _fixture.Create<CreateApplicationRequest>();
            createRequest.MainApplicant.ContactInformation.EmailAddress = emailAddress;

            // Act
            var application = _classUnderTest.CreateNewApplication(createRequest);

            // Assert
            Assert.AreEqual(application.Reference, _hashHelper.Generate(emailAddress).Substring(0, 10), "Reference is incorrect");
        }
    }
}
