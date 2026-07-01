using FluentAssertions;
using HousingRegisterApi.V1.Infrastructure;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    public class AuthEmailValidatorTests
    {
        [TestCase("resident@hackney.gov.uk")]
        [TestCase("user.name+tag@example.com")]
        [TestCase("  resident@hackney.gov.uk  ")]
        public void IsValidReturnsTrueForValidEmailAddresses(string email)
        {
            AuthEmailValidator.IsValid(email).Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("not-an-email")]
        [TestCase("' OR 1=1 --")]
        [TestCase("scanner@prbly.win'; DROP TABLE users; --")]
        [TestCase("@missing-local-part.com")]
        [TestCase("missing-domain@")]
        public void IsValidReturnsFalseForInvalidEmailAddresses(string email)
        {
            AuthEmailValidator.IsValid(email).Should().BeFalse();
        }

        [Test]
        public void IsValidReturnsFalseWhenEmailExceedsMaxLength()
        {
            var localPart = new string('a', 250);
            var email = $"{localPart}@example.com";

            AuthEmailValidator.IsValid(email).Should().BeFalse();
        }
    }
}
