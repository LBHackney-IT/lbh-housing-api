using FluentAssertions;
using HousingRegisterApi.V1.Infrastructure;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    public class AuthEmailMatcherTests
    {
        [TestCase("resident@hackney.gov.uk", null, true)]
        [TestCase("resident@hackney.gov.uk", "", true)]
        [TestCase("resident@hackney.gov.uk", "   ", true)]
        [TestCase("resident@gmail.com", "*", true)]
        [TestCase("resident@hackney.gov.uk", "hackney.gov.uk", true)]
        [TestCase("RESIDENT@Hackney.gov.uk", "hackney.gov.uk", true)]
        [TestCase("resident@hackney.gov.uk", "*@hackney.gov.uk", true)]
        [TestCase("resident@gmail.com", "*@hackney.gov.uk", false)]
        [TestCase("resident@staff.hackney.gov.uk", "*@hackney.gov.uk", false)]
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
            AuthEmailMatcher.IsAllowed(email, allowedDomainsConfig)
                .Should()
                .Be(expected);
        }

        [TestCase("scanner@prbly.win", "*@prbly.win", true)]
        [TestCase("other@prbly.win", "*@prbly.win", true)]
        [TestCase("SCANNER@prbly.win", "*@prbly.win", true)]
        [TestCase("resident@hackney.gov.uk", "*@prbly.win", false)]
        [TestCase("scanner@prbly.win", "scanner@prbly.win", false)]
        [TestCase("resident@hackney.gov.uk", "", false)]
        [TestCase("resident@hackney.gov.uk", null, false)]
        [TestCase("anyone@prbly.winxyzabcdefghij", "*@prbly.*", true)]
        [TestCase("anyone@prbly.win", "*@prbly.*", true)]
        [TestCase("anyone@prblywin.com", "*@prbly.*", false)]
        [TestCase("anyone@mail.prbly.win", "*@prbly.*", false)]
        [TestCase("anyone@prbly.winxyzabcdefghij", "*@prbly*", true)]
        [TestCase("anyone@prblywin.com", "*@prbly*", true)]
        [TestCase("anyone@notprbly.win", "*@prbly*", false)]
        [TestCase("resident@hackney.gov.uk", "*@prbly.*,*@prbly.win", false)]
        public void IsBlockedMatchesConfiguredRules(
            string email,
            string blockedEmailsConfig,
            bool expected)
        {
            AuthEmailMatcher.IsBlocked(email, blockedEmailsConfig)
                .Should()
                .Be(expected);
        }
    }
}
