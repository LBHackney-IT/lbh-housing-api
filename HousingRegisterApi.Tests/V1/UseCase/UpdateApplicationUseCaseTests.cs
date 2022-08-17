using AutoFixture;
using FluentAssertions;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Boundary.Response.Exceptions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class UpdateApplicationUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockGateway;
        private Mock<IActivityGateway> _mockActivityGateway;
        private UpdateApplicationUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _mockActivityGateway = new Mock<IActivityGateway>();
            _classUnderTest = new UpdateApplicationUseCase(_mockGateway.Object, _mockActivityGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task UpdateApplicationCallsGateway()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            _mockGateway
                .Setup(x => x.UpdateApplication(id, It.IsAny<UpdateApplicationRequest>()))
                .Returns(application);

            // Act
            var response = await _classUnderTest.Execute(id, new UpdateApplicationRequest()).ConfigureAwait(false);

            // Assert
            _mockGateway.Verify(x => x.UpdateApplication(id, It.IsAny<UpdateApplicationRequest>()));
            response.Should().BeEquivalentTo(application.ToResponse());
        }

        [Test]
        public async Task UpdateApplicationLogsActivites()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();

            _mockGateway
                .Setup(x => x.GetApplicationById(id))
                .Returns(application);

            _mockGateway
                .Setup(x => x.UpdateApplication(id, It.IsAny<UpdateApplicationRequest>()))
                .Returns(application);

            var response = await _classUnderTest.Execute(id, new UpdateApplicationRequest()
            {
                Status = ApplicationStatus.New,
                Assessment = new AssessmentRequest
                {
                    Band = application.Assessment.Band,
                    BedroomNeed = application.Assessment.BedroomNeed,
                    BiddingNumber = application?.Assessment?.BiddingNumber?.ToString(),
                    EffectiveDate = application.Assessment.EffectiveDate,
                    GenerateBiddingNumber = application.Assessment.GenerateBiddingNumber,
                    InformationReceivedDate = application.Assessment.InformationReceivedDate,
                    Reason = application.Assessment.Reason
                }
            }).ConfigureAwait(false);

            // Assert
            _mockActivityGateway.Verify(x => x.LogActivity(It.IsAny<Application>(), It.IsAny<EntityActivity<ApplicationActivityType>>()));
        }

        [Test]
        public async Task AutoGenerateApplicationNumberWorks()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            var assessment = application.Assessment;
            application.Assessment = null;

            assessment.BiddingNumber = null;
            assessment.GenerateBiddingNumber = true;

            _mockGateway
                .Setup(x => x.GetApplicationById(id))
                .Returns(application);

            _mockGateway
                .Setup(x => x.UpdateApplication(id, It.IsAny<UpdateApplicationRequest>()))
                .Returns<Guid, UpdateApplicationRequest>((g, a) =>
                {
                    return new Application
                    {
                        Assessment = a.Assessment.ToDomain(),
                        AssignedTo = a.AssignedTo,
                        MainApplicant = a.MainApplicant,
                        OtherMembers = a.OtherMembers,
                        Status = a.Status
                    };
                });

            _mockGateway
                .Setup(x => x.GetLastIssuedBiddingNumber())
                .ReturnsAsync(700);

            _mockGateway
                .Setup(x => x.IssueNextBiddingNumber())
                .ReturnsAsync(701);

            //Act
            var response = await _classUnderTest.Execute(id, new UpdateApplicationRequest()
            {
                Status = ApplicationStatus.New,
                Assessment = new AssessmentRequest
                {
                    Band = assessment.Band,
                    BedroomNeed = assessment.BedroomNeed,
                    BiddingNumber = assessment.BiddingNumber?.ToString(),
                    EffectiveDate = assessment.EffectiveDate,
                    GenerateBiddingNumber = assessment.GenerateBiddingNumber,
                    InformationReceivedDate = assessment.InformationReceivedDate,
                    Reason = assessment.Reason
                }
            }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual("701", response.Assessment.BiddingNumber, "Bidding number was auto generated");
        }

        [Test]
        public void ManuallyEnteredBiddingNumberIsInvalidatedIfAboveAutogeneratedNumber()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            var assessment = application.Assessment;
            application.Assessment = null;

            assessment.BiddingNumber = null;
            assessment.GenerateBiddingNumber = false;
            assessment.BiddingNumber = 705;

            _mockGateway
                .Setup(x => x.GetApplicationById(id))
                .Returns(application);

            _mockGateway
                .Setup(x => x.UpdateApplication(id, It.IsAny<UpdateApplicationRequest>()))
                .Returns<Guid, UpdateApplicationRequest>((g, a) =>
                {
                    return new Application
                    {
                        Assessment = a.Assessment.ToDomain(),
                        AssignedTo = a.AssignedTo,
                        MainApplicant = a.MainApplicant,
                        OtherMembers = a.OtherMembers,
                        Status = a.Status
                    };
                });

            _mockGateway
                .Setup(x => x.GetLastIssuedBiddingNumber())
                .ReturnsAsync(700);

            _mockGateway
                .Setup(x => x.IssueNextBiddingNumber())
                .ReturnsAsync(701);

            //Act & Assert
            Assert.ThrowsAsync<InvalidBiddingNumberException>(async () => await _classUnderTest.Execute(id, new UpdateApplicationRequest()
            {
                Status = ApplicationStatus.New,
                Assessment = new AssessmentRequest
                {
                    Band = assessment.Band,
                    BedroomNeed = assessment.BedroomNeed,
                    BiddingNumber = assessment.BiddingNumber?.ToString(),
                    EffectiveDate = assessment.EffectiveDate,
                    GenerateBiddingNumber = assessment.GenerateBiddingNumber,
                    InformationReceivedDate = assessment.InformationReceivedDate,
                    Reason = assessment.Reason
                }
            }).ConfigureAwait(false), "Bidding number that is above the last generated number should not be valid!");
        }
    }
}
