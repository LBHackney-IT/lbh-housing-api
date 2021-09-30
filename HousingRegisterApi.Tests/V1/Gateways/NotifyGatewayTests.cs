using AutoFixture;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using Notify.Interfaces;
using System.Collections.Generic;
using Notify.Models.Responses;

namespace HousingRegisterApi.Tests.V1.Gateways
{
    [TestFixture]
    public class NotifyGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<INotificationClient> _notifyClient;
        private NotifyGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _notifyClient = new Mock<INotificationClient>();
            _classUnderTest = new NotifyGateway(_notifyClient.Object);
        }

        [Test]
        public void CanSendVerifyCodeEmail()
        {
            // Arrange
            var expectedTemplateId = Environment.GetEnvironmentVariable("NOTIFY_TEMPLATE_VERIFY_CODE");
            var application = _fixture.Create<Application>();
            var notifyResponse = _fixture.Create<EmailNotificationResponse>();
            _notifyClient
                .Setup(x => x.SendEmail(It.IsAny<string>(), expectedTemplateId, It.IsAny<Dictionary<string, object>>(), null, null))
                .Returns(notifyResponse);

            // Act
            var response = _classUnderTest.SendVerifyCode(application.MainApplicant, application.VerifyCode);

            // Assert
            response.Should().NotBeNull();
            _notifyClient.Verify(x => x.SendEmail(application.MainApplicant.ContactInformation.EmailAddress, expectedTemplateId, It.IsAny<Dictionary<string, object>>(), null, null), Times.Once);
        }
    }
}
