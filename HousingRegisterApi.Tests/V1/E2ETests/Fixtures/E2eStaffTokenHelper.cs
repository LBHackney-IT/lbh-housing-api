using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HousingRegisterApi.Tests.V1.E2ETests.Fixtures
{
    /// <summary>
    /// Builds Hackney staff JWTs for E2E tests using the same claim shape as LBH Google auth.
    /// </summary>
    public static class E2eStaffTokenHelper
    {
        public const string StaffEmail = "e2e-testing@development.com";
        public const string StaffName = "Tester";
        public const string StaffGroup = "e2e-testing";
        public const string TestJwtSecret = "e2e-test-only-jwt-secret";

        public static string CreateStaffAuthorizationHeaderValue(string group = StaffGroup)
        {
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TestJwtSecret)),
                SecurityAlgorithms.HmacSha256Signature);
            var tokenHandler = new JwtSecurityTokenHandler();

            var payload = new JwtPayload(
                issuer: "Hackney",
                audience: null,
                claims: new List<Claim>
                {
                    new Claim("sub", "115018116092098676113"),
                    new Claim("email", StaffEmail),
                    new Claim("name", StaffName),
                },
                notBefore: null,
                expires: null,
                issuedAt: DateTime.UtcNow);
            payload.Add("groups", new[] { group });

            var token = new JwtSecurityToken(new JwtHeader(signingCredentials), payload);

            return tokenHandler.WriteToken(token);
        }
    }
}
