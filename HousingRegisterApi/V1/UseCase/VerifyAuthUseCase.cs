using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace HousingRegisterApi.V1.UseCase
{
    public class VerifyAuthUseCase : IVerifyAuthUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        public VerifyAuthUseCase(IApplicationApiGateway gateway)
        {
            _gateway = gateway;
        }

        public VerifyAuthResponse Execute(Guid id, VerifyAuthRequest request)
        {
            var response = _gateway.ConfirmVerifyCode(id, request);
            if (response == null)
            {
                return null;
            }

            var accessToken = GenerateJwtToken(response.Id);
            return new VerifyAuthResponse()
            {
                AccessToken = accessToken
            };
        }

        // TODO: move to separate class
        private static string GenerateJwtToken(Guid id)
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
    }
}
