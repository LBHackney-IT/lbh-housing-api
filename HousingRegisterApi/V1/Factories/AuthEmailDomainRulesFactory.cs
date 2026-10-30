using HousingRegisterApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Factories
{
    /// <summary>
    /// Reads the comma-separated ALLOWED_AUTH_EMAIL_DOMAINS and BLOCKED_AUTH_EMAILS settings.
    /// Entries that don't fit the supported formats are ignored.
    /// </summary>
    public static class AuthEmailDomainRulesFactory
    {
        private const string Wildcard = "*";
        private const string AtSign = "@";
        private const string AnyLocalPart = "*@";

        public static AuthEmailDomainRules ToAuthEmailDomainRules(this ApiOptions options)
        {
            var allowedEntries = SplitEntries(options.AllowedAuthEmailDomains);
            var blockedDomainPatterns = SplitEntries(options.BlockedAuthEmails)
                .Where(entry => entry.StartsWith(AnyLocalPart, StringComparison.Ordinal))
                .Select(entry => RemovePrefix(entry, AnyLocalPart))
                .ToList();

            return new AuthEmailDomainRules(
                allowsAnyDomain: !allowedEntries.Any() || allowedEntries.Contains(Wildcard),
                allowedDomains: allowedEntries.Select(ToAllowedDomain).Where(IsPlainDomain),
                blockedDomains: blockedDomainPatterns.Where(IsPlainDomain),
                blockedDomainPrefixes: blockedDomainPatterns.Select(ToBlockedDomainPrefix).Where(IsPlainDomain));
        }

        /// <summary>
        /// "hackney.gov.uk", "@hackney.gov.uk" and "*@hackney.gov.uk" all mean "hackney.gov.uk".
        /// </summary>
        private static string ToAllowedDomain(string entry)
        {
            if (entry.StartsWith(AnyLocalPart, StringComparison.Ordinal))
            {
                return RemovePrefix(entry, AnyLocalPart);
            }

            if (entry.StartsWith(AtSign, StringComparison.Ordinal))
            {
                return RemovePrefix(entry, AtSign);
            }

            return entry;
        }

        /// <summary>
        /// "prbly*" means the prefix "prbly". Anything not ending in a wildcard has no prefix.
        /// </summary>
        private static string ToBlockedDomainPrefix(string domainPattern)
        {
            if (!domainPattern.EndsWith(Wildcard, StringComparison.Ordinal))
            {
                return null;
            }

            return RemoveSuffix(domainPattern, Wildcard);
        }

        private static bool IsPlainDomain(string domain)
        {
            return !string.IsNullOrEmpty(domain)
                && !domain.Contains(AtSign)
                && !domain.Contains(Wildcard);
        }

        private static string RemovePrefix(string value, string prefix)
        {
            return value[prefix.Length..];
        }

        private static string RemoveSuffix(string value, string suffix)
        {
            return value[..^suffix.Length];
        }

        private static List<string> SplitEntries(string config)
        {
            return (config ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(entry => entry.ToLowerInvariant())
                .ToList();
        }
    }
}
