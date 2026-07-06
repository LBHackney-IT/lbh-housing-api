using Hackney.Core.Http;
using Hackney.Core.JWT;
using HousingRegisterApi.V1;
using HousingRegisterApi.V1.Boundary.Response.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Infrastructure
{
    public static class StaffAuthValidator
    {
        public static void EnsureCanCreateApplication(
            HttpContext context,
            IHttpContextWrapper contextWrapper,
            ITokenFactory tokenFactory,
            ApiOptions apiOptions)
        {
            if (!TryGetStaffToken(context, contextWrapper, tokenFactory, out var token))
            {
                throw new UnauthorizedStaffException();
            }

            var allowedGroups = apiOptions.GetStaffGroupsAllowedToCreateApplications().ToList();
            if (!allowedGroups.Any())
            {
                throw new UnauthorizedStaffException();
            }

            if (token.Groups == null ||
                !token.Groups.Any(group => allowedGroups.Contains(group, StringComparer.OrdinalIgnoreCase)))
            {
                throw new UnauthorizedStaffException();
            }
        }

        private static bool TryGetStaffToken(
            HttpContext context,
            IHttpContextWrapper contextWrapper,
            ITokenFactory tokenFactory,
            out Token token)
        {
            token = null;

            try
            {
                token = tokenFactory.Create(contextWrapper.GetContextRequestHeaders(context));
            }
            catch (ArgumentException)
            {
                return false;
            }

            return token != null;
        }
    }
}
