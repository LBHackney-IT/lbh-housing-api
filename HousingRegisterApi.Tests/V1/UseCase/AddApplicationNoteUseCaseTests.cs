using FluentAssertions;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using HousingRegisterApi.V1;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response.Exceptions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class AddApplicationNoteUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockGateway;
        private Mock<IActivityGateway> _mockActivityGateway;
        private Mock<ITokenFactory> _mockTokenFactory;
        private AddApplicationNoteUseCase _classUnderTest;
        private Guid _id;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _mockActivityGateway = new Mock<IActivityGateway>();
            _mockTokenFactory = new Mock<ITokenFactory>();
            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            var mockContextWrapper = new Mock<IHttpContextWrapper>();
            var apiOptions = new ApiOptions
            {
                AuthorisedOfficerGroup = "officer-group",
                AuthorisedReadOnlyGroup = "read-only-group",
            };

            var httpContext = new DefaultHttpContext();
            mockContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            mockContextWrapper
                .Setup(x => x.GetContextRequestHeaders(httpContext))
                .Returns(httpContext.Request.Headers);

            _classUnderTest = new AddApplicationNoteUseCase(
                _mockGateway.Object,
                _mockActivityGateway.Object,
                mockContextAccessor.Object,
                mockContextWrapper.Object,
                _mockTokenFactory.Object,
                apiOptions);

            _id = Guid.NewGuid();
            _mockGateway
                .Setup(x => x.GetApplicationById(_id))
                .Returns(new Application { Id = _id, Status = ApplicationStatus.Submitted });
        }

        private void CallerHasGroups(params string[] groups)
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

        private void AssertNothingWasRecorded()
        {
            _mockActivityGateway.Verify(
                x => x.LogActivity(It.IsAny<Application>(), It.IsAny<EntityActivity<ApplicationActivityType>>()),
                Times.Never);
        }

        [Test]
        public void StaffWithWriteAccessCanAddANote()
        {
            CallerHasGroups("officer-group");

            var response = _classUnderTest.Execute(_id, new AddApplicationNoteRequest { Note = "Called the applicant" });

            response.Should().NotBeNull();
            _mockActivityGateway.Verify(
                x => x.LogActivity(
                    It.Is<Application>(a => a.Id == _id),
                    It.Is<EntityActivity<ApplicationActivityType>>(e =>
                        e.ActivityType == ApplicationActivityType.NoteAddedByUser
                        && e.NewData.ContainsValue("Called the applicant"))),
                Times.Once);
        }

        [Test]
        public void ResidentsCannotAddANote()
        {
            CallerIsResident();

            Action act = () => _classUnderTest.Execute(_id, new AddApplicationNoteRequest { Note = "Hello" });

            act.Should().Throw<UnauthorizedStaffException>();
            AssertNothingWasRecorded();
        }

        [Test]
        public void ReadOnlyStaffCannotAddANote()
        {
            CallerHasGroups("read-only-group");

            Action act = () => _classUnderTest.Execute(_id, new AddApplicationNoteRequest { Note = "Hello" });

            act.Should().Throw<UnauthorizedStaffException>();
            AssertNothingWasRecorded();
        }

        [Test]
        public void UnparseableTokenCannotAddANote()
        {
            _mockTokenFactory
                .Setup(x => x.Create(It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
                .Throws(new ArgumentException("bad token"));

            Action act = () => _classUnderTest.Execute(_id, new AddApplicationNoteRequest { Note = "Hello" });

            act.Should().Throw<UnauthorizedStaffException>();
            AssertNothingWasRecorded();
        }

        [Test]
        public void ReturnsNullWhenTheApplicationDoesNotExist()
        {
            CallerHasGroups("officer-group");
            var missing = Guid.NewGuid();
            _mockGateway.Setup(x => x.GetApplicationById(missing)).Returns((Application) null);

            var response = _classUnderTest.Execute(missing, new AddApplicationNoteRequest { Note = "Hello" });

            response.Should().BeNull();
            AssertNothingWasRecorded();
        }
    }
}
