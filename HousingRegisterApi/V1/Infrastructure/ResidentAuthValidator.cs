using Hackney.Core.Http;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace HousingRegisterApi.V1.Infrastructure
{
    public static class ResidentAuthValidator
    {
        /// <summary>
        /// Header the BFF uses to pass the resident's own token (the housing_user cookie) on.
        /// </summary>
        public const string ResidentTokenHeader = "X-Resident-Token";

        private const string ApplicationIdClaim = "application_id";

        /// <summary>
        /// True when the request carries a valid resident token for the given application.
        /// Missing, expired, tampered and staff cognito tokens are all rejected.
        /// </summary>
        public static bool IsResidentForApplication(
            HttpContext context,
            IHttpContextWrapper contextWrapper,
            ITokenGenerator tokenGenerator,
            Guid applicationId)
        {
            if (context == null)
            {
                return false;
            }

            var headers = contextWrapper.GetContextRequestHeaders(context);
            if (!headers.TryGetValue(ResidentTokenHeader, out var values))
            {
                return false;
            }

            var token = values.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            if (!tokenGenerator.ValidateToken(token, out var claims))
            {
                return false;
            }

            return claims.Any(claim =>
                claim.Type == ApplicationIdClaim
                && string.Equals(claim.Value, applicationId.ToString(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
