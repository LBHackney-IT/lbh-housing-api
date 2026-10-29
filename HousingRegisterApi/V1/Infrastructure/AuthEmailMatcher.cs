using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Infrastructure
{
    public static class AuthEmailMatcher
    {
        public static bool IsAllowed(string email, string allowedDomainsConfig)
        {
            if (string.IsNullOrWhiteSpace(allowedDomainsConfig))
            {
                return true;
            }

            var rules = ParseRules(allowedDomainsConfig);
            if (rules.Contains("*"))
            {
                return true;
            }

            var domains = rules
                .Select(TryParseAllowedDomain)
                .Where(domain => domain != null)
                .ToList();

            return TryGetDomain(email, out var emailDomain) && domains.Contains(emailDomain);
        }

        public static bool IsBlocked(string email, string blockedEmailsConfig)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(blockedEmailsConfig))
            {
                return false;
            }

            return ParseRules(blockedEmailsConfig).Any(rule => MatchesBlockedDomain(email, rule));
        }

        private static IReadOnlyList<string> ParseRules(string config)
        {
            return config
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(rule => rule.Trim().ToLowerInvariant())
                .Where(rule => rule.Length > 0)
                .ToList();
        }

        // *@prbly.win matches that domain. *@prbly* matches a domain starting with
        // "prbly". *@prbly.* matches a domain starting with "prbly.".
        private static bool MatchesBlockedDomain(string email, string rule)
        {
            if (!rule.StartsWith("*@", StringComparison.Ordinal) || !TryGetDomain(email, out var domain))
            {
                return false;
            }

            var domainRule = rule[2..];
            if (domainRule.Length > 1 && domainRule[^1] == '*' && domainRule.IndexOf('*') == domainRule.Length - 1)
            {
                var prefix = domainRule[..^1];
                return domain.Length > prefix.Length
                    && domain.StartsWith(prefix, StringComparison.Ordinal);
            }

            return !domainRule.Contains('*') && domain == domainRule;
        }

        private static string TryParseAllowedDomain(string rule)
        {
            if (rule.StartsWith("*@", StringComparison.Ordinal))
            {
                rule = rule[2..];
            }
            else if (rule.StartsWith('@'))
            {
                rule = rule[1..];
            }

            if (rule.Length == 0 || rule.Contains('@') || rule.Contains('*'))
            {
                return null;
            }

            return rule;
        }

        private static bool TryGetDomain(string value, out string domain)
        {
            domain = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalized = value.Trim().ToLowerInvariant();
            var atIndex = normalized.LastIndexOf('@');
            if (atIndex <= 0 || atIndex == normalized.Length - 1)
            {
                return false;
            }

            domain = normalized[(atIndex + 1)..];
            return true;
        }
    }
}
