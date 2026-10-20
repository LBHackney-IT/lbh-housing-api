using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Infrastructure
{
    public static class AllowedAuthEmailMatcher
    {
        public static bool IsAllowed(string email, string allowedDomainsConfig)
        {
            if (string.IsNullOrWhiteSpace(allowedDomainsConfig))
            {
                return true;
            }

            var domains = ParseDomains(allowedDomainsConfig).ToList();
            if (domains.Contains("*"))
            {
                return true;
            }

            if (domains.Count == 0 || string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var normalizedEmail = email.Trim().ToLowerInvariant();
            var atIndex = normalizedEmail.LastIndexOf('@');
            if (atIndex < 0 || atIndex == normalizedEmail.Length - 1)
            {
                return false;
            }

            var domain = normalizedEmail[(atIndex + 1)..];
            return domains.Contains(domain);
        }

        private static IEnumerable<string> ParseDomains(string allowedDomainsConfig)
        {
            return allowedDomainsConfig
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(NormalizeDomain)
                .Where(domain => domain != null);
        }

        private static string NormalizeDomain(string rule)
        {
            var value = rule.Trim().ToLowerInvariant();
            if (value.Length == 0)
            {
                return null;
            }

            if (value == "*")
            {
                return value;
            }

            if (value.StartsWith("*@", StringComparison.Ordinal))
            {
                value = value[2..];
            }
            else if (value.StartsWith('@'))
            {
                value = value[1..];
            }

            if (value.Length == 0 || value.Contains('@') || value.Contains('*'))
            {
                return null;
            }

            return value;
        }
    }
}
