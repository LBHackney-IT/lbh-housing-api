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
using HousingRegisterApi.V1.UseCase;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class CreateNewApplicationUseCaseTests
    {
        private Mock<IApplicationApiGateway> _mockGateway;
        private Mock<IActivityGateway> _mockActivityGateway;
        private Mock<IHttpContextAccessor> _mockContextAccessor;
        private Mock<IHttpContextWrapper> _mockContextWrapper;
        private Mock<ITokenFactory> _mockTokenFactory;
        private ApiOptions _apiOptions;
        private CreateNewApplicationUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IApplicationApiGateway>();
            _mockActivityGateway = new Mock<IActivityGateway>();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockContextWrapper = new Mock<IHttpContextWrapper>();
            _mockTokenFactory = new Mock<ITokenFactory>();
            _apiOptions = new ApiOptions
            {
                AuthorisedOfficerGroup = "officer-group",
            };
            _classUnderTest = new CreateNewApplicationUseCase(
                _mockGateway.Object,
                _mockActivityGateway.Object,
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
        }

        [Test]
        public void CreateNewApplicationCallsGatewayWhenStaffTokenIsAuthorized()
        {
            var application = _fixture.Create<Application>();
            _mockTokenFactory
                .Setup(x => x.Create(It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
                .Returns(new Token { Groups = new[] { "officer-group" } });
            _mockGateway
                .Setup(x => x.CreateNewApplication(It.IsAny<CreateApplicationRequest>()))
                .Returns(application);

            var response = _classUnderTest.Execute(new CreateApplicationRequest());

            _mockGateway.Verify(x => x.CreateNewApplication(It.IsAny<CreateApplicationRequest>()));
            response.Should().BeEquivalentTo(application.ToResponse());
        }

        [Test]
        public void CreateNewApplicationThrowsWhenStaffTokenIsMissing()
        {
            _mockTokenFactory
                .Setup(x => x.Create(It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
                .Returns((Token)null);

            Action act = () => _classUnderTest.Execute(new CreateApplicationRequest());

            act.Should().Throw<UnauthorizedStaffException>();
            _mockGateway.Verify(
                x => x.CreateNewApplication(It.IsAny<CreateApplicationRequest>()),
                Times.Never);
        }

        [Test]
        public void CreateNewApplicationThrowsWhenStaffTokenIsReadOnly()
        {
            _apiOptions.AuthorisedReadOnlyGroup = "read-only-group";
            _mockTokenFactory
                .Setup(x => x.Create(It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
                .Returns(new Token { Groups = new[] { "read-only-group" } });

            Action act = () => _classUnderTest.Execute(new CreateApplicationRequest());

            act.Should().Throw<UnauthorizedStaffException>();
            _mockGateway.Verify(
                x => x.CreateNewApplication(It.IsAny<CreateApplicationRequest>()),
                Times.Never);
        }

        [Test]
        public void CreateNewApplicationExceptionIsThrown()
        {
            _mockTokenFactory
                .Setup(x => x.Create(It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
                .Returns(new Token { Groups = new[] { "officer-group" } });

            var exception = new ApplicationException("Test exception");
            _mockGateway
                .Setup(x => x.CreateNewApplication(It.IsAny<CreateApplicationRequest>()))
                .Throws(exception);

            Func<ApplicationResponse> func = () => _classUnderTest.Execute(new CreateApplicationRequest());

            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
