using FluentAssertions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Services;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Services
{
    public class AuthEmailDomainServiceTests
    {
        private static AuthEmailDomainService ServiceWith(
            bool allowsAnyDomain = true,
            string[] allowedDomains = null,
            string[] blockedDomains = null,
            string[] blockedDomainPrefixes = null)
        {
            return new AuthEmailDomainService(new AuthEmailDomainRules(
                allowsAnyDomain,
                allowedDomains ?? new string[0],
                blockedDomains ?? new string[0],
                blockedDomainPrefixes ?? new string[0]));
        }

        [Test]
        public void AllowsAnyDomainWhenUnrestricted()
        {
            ServiceWith(allowsAnyDomain: true)
                .CanRequestVerificationCode("resident@gmail.com")
                .Should().BeTrue();
        }

        [TestCase("resident@hackney.gov.uk", true)]
        [TestCase("resident@lbhackney.gov.uk", true)]
        [TestCase("resident@gmail.com", false)]
        [TestCase("resident@not-hackney.gov.uk", false)]
        [TestCase("resident@hackney.gov.uk.evil.com", false)]
        [TestCase("resident@staff.hackney.gov.uk", false)]
        public void OnlyAllowsExactAllowedDomains(string email, bool expected)
        {
            ServiceWith(allowsAnyDomain: false, allowedDomains: new[] { "hackney.gov.uk", "lbhackney.gov.uk" })
                .CanRequestVerificationCode(email)
                .Should().Be(expected);
        }

        [Test]
        public void RejectsEverythingWhenRestrictedToNoDomains()
        {
            ServiceWith(allowsAnyDomain: false)
                .CanRequestVerificationCode("resident@hackney.gov.uk")
                .Should().BeFalse();
        }

        [TestCase("scanner@prbly.win", false)]
        [TestCase("resident@hackney.gov.uk", true)]
        [TestCase("resident@mail.prbly.win", true)]
        public void RejectsExactBlockedDomains(string email, bool expected)
        {
            ServiceWith(blockedDomains: new[] { "prbly.win" })
                .CanRequestVerificationCode(email)
                .Should().Be(expected);
        }

        [TestCase("anyone@prbly.win", false)]
        [TestCase("anyone@prbly.winxyzabcdefghij", false)]
        [TestCase("anyone@prblywin.com", true)]
        [TestCase("anyone@mail.prbly.win", true)]
        public void RejectsDomainsStartingWithBlockedPrefixIncludingDot(string email, bool expected)
        {
            ServiceWith(blockedDomainPrefixes: new[] { "prbly." })
                .CanRequestVerificationCode(email)
                .Should().Be(expected);
        }

        [TestCase("anyone@prblywin.com", false)]
        [TestCase("anyone@prbly.win", false)]
        [TestCase("anyone@prbly", true)]
        [TestCase("anyone@notprbly.win", true)]
        public void RejectsDomainsLongerThanAndStartingWithBlockedPrefix(string email, bool expected)
        {
            ServiceWith(blockedDomainPrefixes: new[] { "prbly" })
                .CanRequestVerificationCode(email)
                .Should().Be(expected);
        }

        [Test]
        public void BlockedDomainsOverrideAllowedDomains()
        {
            ServiceWith(
                    allowsAnyDomain: false,
                    allowedDomains: new[] { "prbly.win" },
                    blockedDomains: new[] { "prbly.win" })
                .CanRequestVerificationCode("scanner@prbly.win")
                .Should().BeFalse();
        }
    }
}
