using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace HousingRegisterApi.V1.Infrastructure
{
    public interface ITokenGenerator
    {
        public string GenerateTokenForApplication(Guid id);

        public IEnumerable<Claim> ValidateTokenAndGetClaims(string accessToken);
    }
}
