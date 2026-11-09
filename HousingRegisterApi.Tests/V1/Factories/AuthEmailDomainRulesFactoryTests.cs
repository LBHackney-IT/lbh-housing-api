using FluentAssertions;
using HousingRegisterApi.V1;
using HousingRegisterApi.V1.Factories;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Factories
{
    public class AuthEmailDomainRulesFactoryTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("*")]
        [TestCase("hackney.gov.uk,*")]
        public void AllowsAnyDomainWhenAllowListIsUnsetOrWildcard(string allowedConfig)
        {
            var rules = new ApiOptions { AllowedAuthEmailDomains = allowedConfig }.ToAuthEmailDomainRules();

            rules.AllowsAnyDomain.Should().BeTrue();
        }

        [TestCase("hackney.gov.uk")]
        [TestCase("@hackney.gov.uk")]
        [TestCase("*@hackney.gov.uk")]
        [TestCase(" Hackney.GOV.uk ")]
        public void ReadsEachSupportedAllowedDomainFormat(string allowedConfig)
        {
            var rules = new ApiOptions { AllowedAuthEmailDomains = allowedConfig }.ToAuthEmailDomainRules();

            rules.AllowsAnyDomain.Should().BeFalse();
            rules.AllowedDomains.Should().BeEquivalentTo("hackney.gov.uk");
        }

        [Test]
        public void ReadsMultipleAllowedDomains()
        {
            var rules = new ApiOptions { AllowedAuthEmailDomains = "hackney.gov.uk,lbhackney.gov.uk" }
                .ToAuthEmailDomainRules();

            rules.AllowedDomains.Should().BeEquivalentTo("hackney.gov.uk", "lbhackney.gov.uk");
        }

        [TestCase("not-a-domain@")]
        [TestCase("resident@hackney.gov.uk")]
        [TestCase("*.hackney.gov.uk")]
        public void IgnoresUnsupportedAllowedEntriesWithoutAllowingAnyDomain(string allowedConfig)
        {
            var rules = new ApiOptions { AllowedAuthEmailDomains = allowedConfig }.ToAuthEmailDomainRules();

            rules.AllowsAnyDomain.Should().BeFalse();
            rules.AllowedDomains.Should().BeEmpty();
        }

        [Test]
        public void ReadsBlockedDomainsAndPrefixes()
        {
            var rules = new ApiOptions { BlockedAuthEmails = "*@prbly.win, *@PRBLY*, *@prbly.*" }
                .ToAuthEmailDomainRules();

            rules.BlockedDomains.Should().BeEquivalentTo("prbly.win");
            rules.BlockedDomainPrefixes.Should().BeEquivalentTo("prbly", "prbly.");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("scanner@prbly.win")]
        [TestCase("prbly.win")]
        [TestCase("*@*")]
        [TestCase("*@pr*bly*")]
        [TestCase("*@*.prbly.win")]
        public void IgnoresUnsupportedBlockedEntries(string blockedConfig)
        {
            var rules = new ApiOptions { BlockedAuthEmails = blockedConfig }.ToAuthEmailDomainRules();

            rules.BlockedDomains.Should().BeEmpty();
            rules.BlockedDomainPrefixes.Should().BeEmpty();
        }
    }
}
