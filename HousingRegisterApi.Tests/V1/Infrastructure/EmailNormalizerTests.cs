using FluentAssertions;
using HousingRegisterApi.V1.Infrastructure;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    public class EmailNormalizerTests
    {
        [TestCase("resident@example.com", "resident@example.com")]
        [TestCase("Resident@Example.com", "resident@example.com")]
        [TestCase("  resident@example.com ", "resident@example.com")]
        public void IsSameAddressIgnoresCaseAndSurroundingWhitespace(string stored, string requested)
        {
            EmailNormalizer.IsSameAddress(stored, requested).Should().BeTrue();
        }

        [TestCase("resident@example.com", "other@example.com")]
        [TestCase("resident@example.com", "resident@example.co.uk")]
        public void IsSameAddressRejectsDifferentAddresses(string stored, string requested)
        {
            EmailNormalizer.IsSameAddress(stored, requested).Should().BeFalse();
        }

        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("   ", "   ")]
        [TestCase(null, "resident@example.com")]
        [TestCase("resident@example.com", null)]
        public void IsSameAddressNeverMatchesBlankValues(string stored, string requested)
        {
            EmailNormalizer.IsSameAddress(stored, requested).Should().BeFalse();
        }
    }
}
