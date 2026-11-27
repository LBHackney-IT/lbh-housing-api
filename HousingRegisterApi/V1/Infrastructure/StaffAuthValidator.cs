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
            if (!IsStaffWithWriteAccess(context, contextWrapper, tokenFactory, apiOptions))
            {
                throw new UnauthorizedStaffException();
            }
        }

        /// <summary>
        /// True when the request carries a valid staff token in one of the groups that may
        /// write applications (admin, manager or officer). Read-only staff, residents (who
        /// reach this API without an Authorization header) and unparseable tokens are not.
        /// </summary>
        public static bool IsStaffWithWriteAccess(
            HttpContext context,
            IHttpContextWrapper contextWrapper,
            ITokenFactory tokenFactory,
            ApiOptions apiOptions)
        {
            if (!TryGetStaffToken(context, contextWrapper, tokenFactory, out var token))
            {
                return false;
            }

            var allowedGroups = apiOptions.GetStaffGroupsAllowedToCreateApplications().ToList();
            if (!allowedGroups.Any())
            {
                return false;
            }

            return token.Groups != null &&
                token.Groups.Any(group => allowedGroups.Contains(group, StringComparer.OrdinalIgnoreCase));
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
