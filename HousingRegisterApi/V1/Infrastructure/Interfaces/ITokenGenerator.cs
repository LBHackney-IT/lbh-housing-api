using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace HousingRegisterApi.V1.Infrastructure
{
    public interface ITokenGenerator
    {
        string GenerateTokenForApplication(Guid id);
        bool ValidateToken(string accessToken, out IEnumerable<Claim> claims);
    }
}
