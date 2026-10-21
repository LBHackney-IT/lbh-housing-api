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
            if (rules.Any(rule => rule == "*"))
            {
                return true;
            }

            var domains = rules
                .Select(TryParseDomain)
                .Where(domain => domain != null)
                .ToList();

            var emailDomain = GetDomain(email);
            return emailDomain != null && domains.Contains(emailDomain);
        }

        public static bool IsBlocked(string email, string blockedEmailsConfig)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(blockedEmailsConfig))
            {
                return false;
            }

            var normalizedEmail = NormalizeEmail(email);
            return ParseRules(blockedEmailsConfig)
                .Any(rule => MatchesBlockedRule(normalizedEmail, rule));
        }

        private static IReadOnlyList<string> ParseRules(string config)
        {
            return config
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(rule => rule.Trim().ToLowerInvariant())
                .Where(rule => rule.Length > 0)
                .ToList();
        }

        private static bool MatchesBlockedRule(string normalizedEmail, string rule)
        {
            if (rule.StartsWith("*@", StringComparison.Ordinal))
            {
                var domain = TryParseDomain(rule);
                return domain != null && GetDomain(normalizedEmail) == domain;
            }

            return normalizedEmail == rule;
        }

        private static string TryParseDomain(string rule)
        {
            if (rule.StartsWith("*@", StringComparison.Ordinal))
            {
                rule = rule[2..];
            }
            else if (rule.StartsWith('@'))
            {
                rule = rule[1..];
            }
            else if (rule.Contains('@') || rule == "*")
            {
                return null;
            }

            if (rule.Length == 0 || rule.Contains('@') || rule.Contains('*'))
            {
                return null;
            }

            return rule;
        }

        private static string GetDomain(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var normalizedEmail = NormalizeEmail(email);
            var atIndex = normalizedEmail.LastIndexOf('@');
            if (atIndex < 0 || atIndex == normalizedEmail.Length - 1)
            {
                return null;
            }

            return normalizedEmail[(atIndex + 1)..];
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
    }
}
