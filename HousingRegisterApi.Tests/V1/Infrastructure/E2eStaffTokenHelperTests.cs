using FluentAssertions;
using Hackney.Core.JWT;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    public class E2eStaffTokenHelperTests
    {
        [Test]
        public void CreateStaffAuthorizationHeaderValueIsDecodedByTokenFactory()
        {
            var tokenFactory = new ServiceCollection()
                .AddLogging()
                .AddTokenFactory()
                .BuildServiceProvider()
                .GetRequiredService<ITokenFactory>();
            var headers = new HeaderDictionary
            {
                { "Authorization", E2eStaffTokenHelper.CreateStaffAuthorizationHeaderValue() },
            };

            var token = tokenFactory.Create(headers);

            token.Should().NotBeNull();
            token.Email.Should().Be(E2eStaffTokenHelper.StaffEmail);
            token.Name.Should().Be(E2eStaffTokenHelper.StaffName);
            token.Groups.Should().Contain(E2eStaffTokenHelper.StaffGroup);
        }
    }
}
