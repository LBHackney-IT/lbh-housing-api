using HousingRegisterApi.V1.Domain;
using System;
using System.Linq;

namespace HousingRegisterApi.V1.Services
{
    public class AuthEmailDomainService : IAuthEmailDomainService
    {
        private readonly AuthEmailDomainRules _rules;

        public AuthEmailDomainService(AuthEmailDomainRules rules)
        {
            _rules = rules;
        }

        public bool CanRequestVerificationCode(string email)
        {
            var domain = DomainOf(email);

            return IsAllowed(domain) && !IsBlocked(domain);
        }

        private bool IsAllowed(string domain)
        {
            return _rules.AllowsAnyDomain || _rules.AllowedDomains.Contains(domain);
        }

        private bool IsBlocked(string domain)
        {
            return _rules.BlockedDomains.Contains(domain) || HasBlockedPrefix(domain);
        }

        private bool HasBlockedPrefix(string domain)
        {
            return _rules.BlockedDomainPrefixes.Any(prefix =>
                domain != prefix && domain.StartsWith(prefix, StringComparison.Ordinal));
        }

        // Callers validate that the email contains exactly one '@'.
        private static string DomainOf(string email)
        {
            return email.Split('@').Last();
        }
    }
}
