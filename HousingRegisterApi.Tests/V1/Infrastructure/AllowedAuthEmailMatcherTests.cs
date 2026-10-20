using FluentAssertions;
using HousingRegisterApi.V1.Infrastructure;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    public class AllowedAuthEmailMatcherTests
    {
        [TestCase("resident@hackney.gov.uk", null, true)]
        [TestCase("resident@hackney.gov.uk", "", true)]
        [TestCase("resident@hackney.gov.uk", "   ", true)]
        [TestCase("resident@gmail.com", "*", true)]
        [TestCase("resident@hackney.gov.uk", "hackney.gov.uk", true)]
        [TestCase("RESIDENT@Hackney.gov.uk", "hackney.gov.uk", true)]
        [TestCase("resident@hackney.gov.uk", "*@hackney.gov.uk", true)]
        [TestCase("resident@hackney.gov.uk", "@hackney.gov.uk", true)]
        [TestCase("resident@lbhackney.gov.uk", "hackney.gov.uk,lbhackney.gov.uk", true)]
        [TestCase("resident@gmail.com", "hackney.gov.uk", false)]
        [TestCase("resident@not-hackney.gov.uk", "hackney.gov.uk", false)]
        [TestCase("resident@hackney.gov.uk.evil.com", "hackney.gov.uk", false)]
        [TestCase("resident@staff.hackney.gov.uk", "hackney.gov.uk", false)]
        [TestCase("resident@gmail.com", "not-a-domain@", false)]
        public void IsAllowedMatchesConfiguredDomains(
            string email,
            string allowedDomainsConfig,
            bool expected)
        {
            AllowedAuthEmailMatcher.IsAllowed(email, allowedDomainsConfig)
                .Should()
                .Be(expected);
        }
    }
}
