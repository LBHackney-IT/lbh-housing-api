using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Infrastructure
{
    public static class BlockedAuthEmailMatcher
    {
        public static bool IsBlocked(string email, string blockedEmailsConfig)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(blockedEmailsConfig))
            {
                return false;
            }

            var normalizedEmail = email.Trim().ToLowerInvariant();
            var rules = ParseRules(blockedEmailsConfig);

            return rules.Any(rule => MatchesRule(normalizedEmail, rule));
        }

        private static IEnumerable<string> ParseRules(string blockedEmailsConfig)
        {
            return blockedEmailsConfig
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(rule => rule.Trim().ToLowerInvariant())
                .Where(rule => !string.IsNullOrEmpty(rule));
        }

        private static bool MatchesRule(string normalizedEmail, string rule)
        {
            if (rule.StartsWith("*@", StringComparison.Ordinal))
            {
                var domain = rule[1..];
                return normalizedEmail.EndsWith(domain, StringComparison.Ordinal);
            }

            return normalizedEmail == rule;
        }
    }
}
