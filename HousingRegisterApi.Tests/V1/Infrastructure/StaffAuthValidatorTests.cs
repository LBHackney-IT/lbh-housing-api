using FluentAssertions;
using Hackney.Core.JWT;
using HousingRegisterApi.V1;
using HousingRegisterApi.V1.Infrastructure;
using NUnit.Framework;
using System.Collections.Generic;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    public class StaffAuthValidatorTests
    {
        [Test]
        public void GetStaffGroupsAllowedToCreateApplicationsExcludesReadOnlyGroup()
        {
            var options = new ApiOptions
            {
                AuthorisedAdminGroup = "admin-group",
                AuthorisedManagerGroup = "manager-group",
                AuthorisedOfficerGroup = "officer-group",
                AuthorisedReadOnlyGroup = "read-only-group",
            };

            options.GetStaffGroupsAllowedToCreateApplications()
                .Should()
                .BeEquivalentTo(new[] { "admin-group", "manager-group", "officer-group" });
        }

        [Test]
        public void GetStaffGroupsAllowedToCreateApplicationsIgnoresBlankValues()
        {
            var options = new ApiOptions
            {
                AuthorisedOfficerGroup = "officer-group",
            };

            options.GetStaffGroupsAllowedToCreateApplications()
                .Should()
                .BeEquivalentTo(new[] { "officer-group" });
        }
    }
}
