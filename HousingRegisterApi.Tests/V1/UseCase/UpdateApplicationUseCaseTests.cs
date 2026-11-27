using AutoFixture;
using FluentAssertions;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using HousingRegisterApi.V1;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Boundary.Response.Exceptions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Gateways.Interfaces;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase;
using Microsoft.AspNetCore.Http;
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
        private Mock<ISearchGateway> _mockASearchGateway;
        private Mock<IHttpContextAccessor> _mockContextAccessor;
        private Mock<IHttpContextWrapper> _mockContextWrapper;
        private Mock<ITokenFactory> _mockTokenFactory;
        private ApiOptions _apiOptions;
        private UpdateApplicationUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _mockActivityGateway = new Mock<IActivityGateway>();
            _mockASearchGateway = new Mock<ISearchGateway>();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockContextWrapper = new Mock<IHttpContextWrapper>();
            _mockTokenFactory = new Mock<ITokenFactory>();
            _apiOptions = new ApiOptions
            {
                AuthorisedOfficerGroup = "officer-group",
                AuthorisedReadOnlyGroup = "read-only-group",
            };
            _classUnderTest = new UpdateApplicationUseCase(
                _mockGateway.Object,
                _mockActivityGateway.Object,
                _mockASearchGateway.Object,
                _mockContextAccessor.Object,
                _mockContextWrapper.Object,
                _mockTokenFactory.Object,
                _apiOptions);
            _fixture = new Fixture();

            var httpContext = new DefaultHttpContext();
            _mockContextAccessor
                .Setup(x => x.HttpContext)
                .Returns(httpContext);
            _mockContextWrapper
                .Setup(x => x.GetContextRequestHeaders(httpContext))
                .Returns(httpContext.Request.Headers);

            // Existing tests exercise the staff path, so the caller is staff unless a test says otherwise
            CallerIsStaff("officer-group");
        }

        private void CallerIsStaff(params string[] groups)
        {
            _mockTokenFactory
                .Setup(x => x.Create(It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
                .Returns(new Token { Groups = groups });
        }

        // Residents reach the API with no Authorization header, so there is no token to parse
        private void CallerIsResident()
        {
            _mockTokenFactory
                .Setup(x => x.Create(It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
                .Returns((Token) null);
        }

        private Application ExistingApplication(string status)
        {
            var application = _fixture.Create<Application>();
            application.FixDates();
            application.Status = status;
            application.Assessment.BiddingNumber = "1234567";
            application.Assessment.GenerateBiddingNumber = false;
            return application;
        }

        private void SetupExistingApplication(Guid id, Application application)
        {
            _mockGateway
                .Setup(x => x.GetApplicationById(id))
                .Returns(application);
            _mockGateway
                .Setup(x => x.UpdateApplication(id, It.IsAny<UpdateApplicationRequest>()))
                .Returns(application);
        }

        [Test]
        public async Task UpdateApplicationCallsGateway()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            application.FixDates();
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
            application.FixDates();
            _mockGateway
                .Setup(x => x.GetApplicationById(id))
                .Returns(application);

            _mockGateway
                .Setup(x => x.UpdateApplication(id, It.IsAny<UpdateApplicationRequest>()))
                .Returns(application);

            var response = await _classUnderTest.Execute(id, new UpdateApplicationRequest()
            {
                Status = ApplicationStatus.New,
                Assessment = application.Assessment
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
            application.FixDates();
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
                        Assessment = a.Assessment,
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
                Assessment = assessment
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
            application.FixDates();
            var assessment = application.Assessment;
            application.Assessment = null;

            assessment.BiddingNumber = null;
            assessment.GenerateBiddingNumber = false;
            assessment.BiddingNumber = "705";

            _mockGateway
                .Setup(x => x.GetApplicationById(id))
                .Returns(application);

            _mockGateway
                .Setup(x => x.UpdateApplication(id, It.IsAny<UpdateApplicationRequest>()))
                .Returns<Guid, UpdateApplicationRequest>((g, a) =>
                {
                    return new Application
                    {
                        Assessment = a.Assessment,
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
                Assessment = assessment
            }).ConfigureAwait(false), "Bidding number that is above the last generated number should not be valid!");

        }

        [Test]
        public void ManuallyEnteredBiddingNumberThatIsNotANumberShouldThrow()
        {
            // Arrange
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            application.FixDates();
            var updatedAssessment = application.Assessment;
            application.Assessment = null;

            updatedAssessment.BiddingNumber = null;
            updatedAssessment.GenerateBiddingNumber = false;
            updatedAssessment.BiddingNumber = "erg";

            _mockGateway
                .Setup(x => x.GetApplicationById(id))
                .Returns(application);

            _mockGateway
                .Setup(x => x.UpdateApplication(id, It.IsAny<UpdateApplicationRequest>()))
                .Returns<Guid, UpdateApplicationRequest>((g, a) =>
                {
                    return new Application
                    {
                        Assessment = a.Assessment,
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
                Assessment = updatedAssessment
            }).ConfigureAwait(false), "Invalid bidding number was validated!");

        }

        [Test]
        public async Task AssignmentOnlyUpdateSucceedsWhenStoredApplicationHasBiddingNumber()
        {
            // Staff assignment sends only AssignedTo. Omitting assessment must not be
            // treated as clearing a bidding number that is already stored.
            var id = Guid.NewGuid();
            var application = _fixture.Create<Application>();
            application.FixDates();
            application.Assessment.BiddingNumber = "1234567";
            application.Assessment.GenerateBiddingNumber = false;

            _mockGateway
                .Setup(x => x.GetApplicationById(id))
                .Returns(application);

            _mockGateway
                .Setup(x => x.UpdateApplication(id, It.IsAny<UpdateApplicationRequest>()))
                .Returns(application);

            var request = new UpdateApplicationRequest
            {
                AssignedTo = "officer@hackney.gov.uk"
            };

            var response = await _classUnderTest.Execute(id, request).ConfigureAwait(false);

            response.Should().BeEquivalentTo(application.ToResponse());
            _mockGateway.Verify(x => x.UpdateApplication(id, request));
            _mockGateway.Verify(x => x.GetLastIssuedBiddingNumber(), Times.Never);
            _mockGateway.Verify(x => x.IssueNextBiddingNumber(), Times.Never);
        }

        [Test]
        public async Task ResidentUpdateKeepsOnlyHouseholdDetails()
        {
            // A resident's browser can hold a stale copy of the whole application, including the
            // status and assessment an officer set earlier. None of that may be written back.
            CallerIsResident();
            var id = Guid.NewGuid();
            var application = ExistingApplication(ApplicationStatus.Submitted);
            SetupExistingApplication(id, application);

            var request = new UpdateApplicationRequest
            {
                Status = ApplicationStatus.Cancelled,
                SensitiveData = true,
                AssignedTo = "someone@hackney.gov.uk",
                Assessment = new Assessment { Reason = "other", Band = "A", BiddingNumber = "9999999" },
                MainApplicant = application.MainApplicant,
                OtherMembers = application.OtherMembers,
            };

            await _classUnderTest.Execute(id, request).ConfigureAwait(false);

            request.Status.Should().BeNull();
            request.SensitiveData.Should().BeNull();
            request.AssignedTo.Should().BeNull();
            request.Assessment.Should().BeNull();
            request.MainApplicant.Should().BeEquivalentTo(application.MainApplicant);
            request.OtherMembers.Should().BeEquivalentTo(application.OtherMembers);
            _mockGateway.Verify(x => x.UpdateApplication(id, request), Times.Once);
        }

        [Test]
        public async Task StaffUpdateCannotChangeTheStoredMainApplicantEmail()
        {
            CallerIsStaff("officer-group");
            var id = Guid.NewGuid();
            var application = ExistingApplication(ApplicationStatus.Submitted);
            application.MainApplicant.ContactInformation = new ContactInformation { EmailAddress = "stored@hackney.gov.uk" };
            SetupExistingApplication(id, application);

            var request = RequestWithMainApplicantEmail(application, "changed@hackney.gov.uk");

            await _classUnderTest.Execute(id, request).ConfigureAwait(false);

            request.MainApplicant.ContactInformation.EmailAddress.Should().Be("stored@hackney.gov.uk");
            _mockGateway.Verify(x => x.UpdateApplication(id, request), Times.Once);
        }

        [Test]
        public async Task ResidentUpdateCannotChangeTheStoredMainApplicantEmail()
        {
            CallerIsResident();
            var id = Guid.NewGuid();
            var application = ExistingApplication(ApplicationStatus.Submitted);
            application.MainApplicant.ContactInformation = new ContactInformation { EmailAddress = "stored@hackney.gov.uk" };
            SetupExistingApplication(id, application);

            var request = RequestWithMainApplicantEmail(application, "changed@hackney.gov.uk");

            await _classUnderTest.Execute(id, request).ConfigureAwait(false);

            request.MainApplicant.ContactInformation.EmailAddress.Should().Be("stored@hackney.gov.uk");
            _mockGateway.Verify(x => x.UpdateApplication(id, request), Times.Once);
        }

        [Test]
        public async Task EmailCanBeSetWhenNoneIsStored()
        {
            CallerIsResident();
            var id = Guid.NewGuid();
            var application = ExistingApplication(ApplicationStatus.Submitted);
            application.MainApplicant.ContactInformation = new ContactInformation { EmailAddress = null };
            SetupExistingApplication(id, application);

            var request = RequestWithMainApplicantEmail(application, "first@hackney.gov.uk");

            await _classUnderTest.Execute(id, request).ConfigureAwait(false);

            request.MainApplicant.ContactInformation.EmailAddress.Should().Be("first@hackney.gov.uk");
        }

        private static UpdateApplicationRequest RequestWithMainApplicantEmail(Application application, string email)
        {
            return new UpdateApplicationRequest
            {
                MainApplicant = new Applicant
                {
                    Person = application.MainApplicant.Person,
                    ContactInformation = new ContactInformation { EmailAddress = email },
                },
            };
        }

        [Test]
        public async Task ResidentUpdateDoesNotLogStatusChangeOrCheckBiddingNumber()
        {
            CallerIsResident();
            var id = Guid.NewGuid();
            var application = ExistingApplication(ApplicationStatus.Submitted);
            SetupExistingApplication(id, application);

            await _classUnderTest.Execute(id, new UpdateApplicationRequest
            {
                Status = ApplicationStatus.Cancelled,
                Assessment = new Assessment { Reason = "other", BiddingNumber = "9999999" },
            }).ConfigureAwait(false);

            _mockActivityGateway.Verify(
                x => x.LogActivity(
                    It.IsAny<Application>(),
                    It.Is<EntityActivity<ApplicationActivityType>>(a => a.ActivityType == ApplicationActivityType.StatusChangedByUser)),
                Times.Never);
            _mockGateway.Verify(x => x.GetLastIssuedBiddingNumber(), Times.Never);
            _mockGateway.Verify(x => x.IssueNextBiddingNumber(), Times.Never);
        }

        [TestCase(ApplicationStatus.New)]
        [TestCase(ApplicationStatus.ManualDraft)]
        public async Task ResidentCanDisqualifyADraftApplication(string storedStatus)
        {
            CallerIsResident();
            var id = Guid.NewGuid();
            var application = ExistingApplication(storedStatus);
            SetupExistingApplication(id, application);

            var request = new UpdateApplicationRequest { Status = ApplicationStatus.Disqualified };

            await _classUnderTest.Execute(id, request).ConfigureAwait(false);

            request.Status.Should().Be(ApplicationStatus.Disqualified);
            _mockActivityGateway.Verify(
                x => x.LogActivity(
                    It.IsAny<Application>(),
                    It.Is<EntityActivity<ApplicationActivityType>>(a => a.ActivityType == ApplicationActivityType.StatusChangedByUser)),
                Times.Once);
        }

        [TestCase(ApplicationStatus.Submitted)]
        [TestCase(ApplicationStatus.Cancelled)]
        [TestCase(ApplicationStatus.Active)]
        [TestCase(ApplicationStatus.Disqualified)]
        public async Task ResidentCannotDisqualifyOnceTheApplicationIsPastDraft(string storedStatus)
        {
            CallerIsResident();
            var id = Guid.NewGuid();
            var application = ExistingApplication(storedStatus);
            SetupExistingApplication(id, application);

            var request = new UpdateApplicationRequest { Status = ApplicationStatus.Disqualified };

            await _classUnderTest.Execute(id, request).ConfigureAwait(false);

            request.Status.Should().BeNull();
        }

        [TestCase(ApplicationStatus.Submitted)]
        [TestCase(ApplicationStatus.Cancelled)]
        [TestCase(ApplicationStatus.Active)]
        [TestCase(ApplicationStatus.New)]
        public async Task ResidentCannotSetAnyOtherStatus(string requestedStatus)
        {
            CallerIsResident();
            var id = Guid.NewGuid();
            var application = ExistingApplication(ApplicationStatus.New);
            SetupExistingApplication(id, application);

            var request = new UpdateApplicationRequest { Status = requestedStatus };

            await _classUnderTest.Execute(id, request).ConfigureAwait(false);

            request.Status.Should().BeNull();
        }

        [Test]
        public async Task ReadOnlyStaffAreTreatedAsResidents()
        {
            CallerIsStaff("read-only-group");
            var id = Guid.NewGuid();
            var application = ExistingApplication(ApplicationStatus.Submitted);
            SetupExistingApplication(id, application);

            var request = new UpdateApplicationRequest
            {
                Status = ApplicationStatus.Active,
                AssignedTo = "someone@hackney.gov.uk",
            };

            await _classUnderTest.Execute(id, request).ConfigureAwait(false);

            request.Status.Should().BeNull();
            request.AssignedTo.Should().BeNull();
        }

        [Test]
        public async Task UnparseableTokenIsTreatedAsResident()
        {
            _mockTokenFactory
                .Setup(x => x.Create(It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
                .Throws(new ArgumentException("bad token"));
            var id = Guid.NewGuid();
            var application = ExistingApplication(ApplicationStatus.Submitted);
            SetupExistingApplication(id, application);

            var request = new UpdateApplicationRequest { Status = ApplicationStatus.Active };

            await _classUnderTest.Execute(id, request).ConfigureAwait(false);

            request.Status.Should().BeNull();
        }

        [Test]
        public async Task StaffUpdateKeepsStatusAssessmentAssignmentAndSensitivity()
        {
            CallerIsStaff("officer-group");
            var id = Guid.NewGuid();
            var application = ExistingApplication(ApplicationStatus.Submitted);
            SetupExistingApplication(id, application);

            var assessment = new Assessment { Reason = "other", Band = "A", BedroomNeed = 2 };
            var request = new UpdateApplicationRequest
            {
                Status = ApplicationStatus.Cancelled,
                SensitiveData = true,
                AssignedTo = "someone@hackney.gov.uk",
                Assessment = assessment,
            };

            await _classUnderTest.Execute(id, request).ConfigureAwait(false);

            request.Status.Should().Be(ApplicationStatus.Cancelled);
            request.SensitiveData.Should().BeTrue();
            request.AssignedTo.Should().Be("someone@hackney.gov.uk");
            request.Assessment.Should().BeSameAs(assessment);
            _mockActivityGateway.Verify(
                x => x.LogActivity(
                    It.IsAny<Application>(),
                    It.Is<EntityActivity<ApplicationActivityType>>(a => a.ActivityType == ApplicationActivityType.StatusChangedByUser)),
                Times.Once);
        }
    }
}
