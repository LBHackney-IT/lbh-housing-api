using FluentAssertions;
using Hackney.Core.Http;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Boundary.Response.Exceptions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class CreateEvidenceRequestUseCaseTests
    {
        private const string ResidentToken = "resident-token";

        private Mock<IApplicationApiGateway> _mockGateway;
        private Mock<IEvidenceApiGateway> _mockEvidenceGateway;
        private Mock<ITokenGenerator> _mockTokenGenerator;
        private DefaultHttpContext _httpContext;
        private CreateEvidenceRequestUseCase _classUnderTest;
        private Guid _id;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _mockEvidenceGateway = new Mock<IEvidenceApiGateway>();
            _mockTokenGenerator = new Mock<ITokenGenerator>();

            _httpContext = new DefaultHttpContext();
            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            mockContextAccessor.Setup(x => x.HttpContext).Returns(_httpContext);
            var mockContextWrapper = new Mock<IHttpContextWrapper>();
            mockContextWrapper
                .Setup(x => x.GetContextRequestHeaders(_httpContext))
                .Returns(_httpContext.Request.Headers);

            _classUnderTest = new CreateEvidenceRequestUseCase(
                _mockGateway.Object,
                _mockEvidenceGateway.Object,
                mockContextAccessor.Object,
                mockContextWrapper.Object,
                _mockTokenGenerator.Object);

            _id = Guid.NewGuid();
            _mockEvidenceGateway
                .Setup(x => x.CreateEvidenceRequest(It.IsAny<CreateEvidenceRequest>()))
                .ReturnsAsync(new EvidenceRequestResponse { Id = Guid.NewGuid() });

            // The default caller is the resident of the application being requested
            PresentResidentToken(_id);
        }

        private void SetupApplication(string status)
        {
            _mockGateway.Setup(x => x.GetApplicationById(_id)).Returns(new Application
            {
                Id = _id,
                Status = status,
                MainApplicant = new Applicant
                {
                    Person = new Person { FirstName = "Ada", Surname = "Lovelace" },
                    ContactInformation = new ContactInformation { EmailAddress = "ada@example.test" },
                },
            });
        }

        // Sends the resident token header, and makes the token validator return the claims for applicationId
        private void PresentResidentToken(Guid applicationId, bool isValid = true)
        {
            _httpContext.Request.Headers[ResidentAuthValidator.ResidentTokenHeader] = ResidentToken;
            IEnumerable<Claim> claims = new[] { new Claim("application_id", applicationId.ToString()) };
            _mockTokenGenerator
                .Setup(x => x.ValidateToken(ResidentToken, out claims))
                .Returns(isValid);
        }

        private void AssertNothingWasLoadedOrSent()
        {
            _mockGateway.Verify(x => x.GetApplicationById(It.IsAny<Guid>()), Times.Never);
            _mockEvidenceGateway.Verify(x => x.CreateEvidenceRequest(It.IsAny<CreateEvidenceRequest>()), Times.Never);
        }

        private static CreateEvidenceRequestBase Request() =>
            new CreateEvidenceRequestBase { DocumentTypes = new List<string> { "proof-of-id" } };

        [TestCase(ApplicationStatus.New)]
        [TestCase(ApplicationStatus.ManualDraft)]
        [TestCase(ApplicationStatus.Cancelled)]
        [TestCase(ApplicationStatus.Disqualified)]
        [TestCase(ApplicationStatus.Active)]
        public void ThrowsAndSendsNothingWhenTheApplicationHasNotBeenSubmitted(string status)
        {
            SetupApplication(status);

            Func<Task> act = () => _classUnderTest.ExecuteAsync(_id, Request());

            act.Should().ThrowAsync<EvidenceRequestNotAllowedException>();
            _mockEvidenceGateway.Verify(x => x.CreateEvidenceRequest(It.IsAny<CreateEvidenceRequest>()), Times.Never);
        }

        [Test]
        public async Task RequestsEvidenceForTheResidentOfASubmittedApplication()
        {
            SetupApplication(ApplicationStatus.Submitted);

            var response = await _classUnderTest.ExecuteAsync(_id, Request()).ConfigureAwait(false);

            response.Should().HaveCount(1);
            _mockEvidenceGateway.Verify(x => x.CreateEvidenceRequest(It.Is<CreateEvidenceRequest>(r =>
                r.DocumentTypes.Contains("proof-of-id") &&
                r.Resident.Email == "ada@example.test" &&
                r.Resident.Name == "Ada Lovelace")), Times.Once);
        }

        [Test]
        public async Task ReturnsNullWhenTheApplicationDoesNotExist()
        {
            _mockGateway.Setup(x => x.GetApplicationById(_id)).Returns((Application) null);

            var response = await _classUnderTest.ExecuteAsync(_id, Request()).ConfigureAwait(false);

            response.Should().BeNull();
            _mockEvidenceGateway.Verify(x => x.CreateEvidenceRequest(It.IsAny<CreateEvidenceRequest>()), Times.Never);
        }

        [Test]
        public void ThrowsWhenNoResidentTokenIsSent()
        {
            _httpContext.Request.Headers.Remove(ResidentAuthValidator.ResidentTokenHeader);
            SetupApplication(ApplicationStatus.Submitted);

            Func<Task> act = () => _classUnderTest.ExecuteAsync(_id, Request());

            act.Should().ThrowAsync<UnauthorizedResidentException>();
            AssertNothingWasLoadedOrSent();
        }

        [Test]
        public void ThrowsWhenTheTokenFailsValidation()
        {
            PresentResidentToken(_id, isValid: false);
            SetupApplication(ApplicationStatus.Submitted);

            Func<Task> act = () => _classUnderTest.ExecuteAsync(_id, Request());

            act.Should().ThrowAsync<UnauthorizedResidentException>();
            AssertNothingWasLoadedOrSent();
        }

        [Test]
        public void ThrowsWhenTheTokenIsForAnotherApplication()
        {
            PresentResidentToken(Guid.NewGuid());
            SetupApplication(ApplicationStatus.Submitted);

            Func<Task> act = () => _classUnderTest.ExecuteAsync(_id, Request());

            act.Should().ThrowAsync<UnauthorizedResidentException>();
            AssertNothingWasLoadedOrSent();
        }
    }
}
