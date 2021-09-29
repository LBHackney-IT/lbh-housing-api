using HousingRegisterApi.V1.Infrastructure;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class VerifyCodeGeneratorTests
    {
        private IVerifyCodeGenerator _classUnderTest;

        [SetUp]
        public void Init()
        {
            _classUnderTest = new VerifyCodeGenerator();
        }

        [Test]
        public void DoesGenerateSixDigitCode()
        {
            // Act
            var verifyCode = _classUnderTest.GenerateCode();

            // Assert
            Assert.IsNotNull(verifyCode);
            Assert.IsTrue(verifyCode.Length == 6);
        }
    }
}
