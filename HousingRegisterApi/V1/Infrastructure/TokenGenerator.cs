using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class TokenGenerator : ITokenGenerator
    {
        public string GenerateTokenForApplication(Guid id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("HACKNEY_JWT_SECRET"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("application_id", id.ToString()),
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddDays(30)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string accessToken, out IEnumerable<Claim> claims)
        {
            claims = new List<Claim>();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("HACKNEY_JWT_SECRET"));

            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                RequireExpirationTime = true,
                ValidateAudience = false,
                ValidateIssuer = false
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(accessToken, validations, out SecurityToken secToken);
                claims = claimsPrincipal.Claims;
                return true; ;
            }
            catch
            {
                return false;
            }
        }
    }
}
