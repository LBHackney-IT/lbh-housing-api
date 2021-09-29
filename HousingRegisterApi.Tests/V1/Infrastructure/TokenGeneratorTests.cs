using System;
using HousingRegisterApi.V1.Infrastructure;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class TokenGeneratorTests
    {
        private ITokenGenerator _classUnderTest;

        [SetUp]
        public void Init()
        {
            _classUnderTest = new TokenGenerator();
            Environment.SetEnvironmentVariable("HACKNEY_JWT_SECRET", "HACKNEY_JWT_SECRET");
        }

        [Test]
        public void DoesGenerateApplicationToken()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var token = _classUnderTest.GenerateTokenForApplication(id);

            // Assert
            Assert.IsNotNull(token);
        }
    }
}
