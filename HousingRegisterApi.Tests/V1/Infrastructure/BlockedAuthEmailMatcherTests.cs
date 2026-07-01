using FluentAssertions;
using HousingRegisterApi.V1.Infrastructure;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    public class BlockedAuthEmailMatcherTests
    {
        [TestCase("scanner@prbly.win", "scanner@prbly.win", true)]
        [TestCase("SCANNER@prbly.win", "scanner@prbly.win", true)]
        [TestCase("scanner@prbly.win", "*@prbly.win", true)]
        [TestCase("other@prbly.win", "*@prbly.win", true)]
        [TestCase("resident@hackney.gov.uk", "scanner@prbly.win,*@prbly.win", false)]
        [TestCase("resident@hackney.gov.uk", "", false)]
        [TestCase("resident@hackney.gov.uk", null, false)]
        public void IsBlockedMatchesConfiguredRules(
            string email,
            string blockedEmailsConfig,
            bool expected)
        {
            BlockedAuthEmailMatcher.IsBlocked(email, blockedEmailsConfig)
                .Should()
                .Be(expected);
        }
    }
}
