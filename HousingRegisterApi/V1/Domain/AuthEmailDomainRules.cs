using System.Collections.Generic;

namespace HousingRegisterApi.V1.Domain
{
    /// <summary>
    /// Which email domains may request a resident verification code.
    /// All domains are lowercase and contain no wildcards.
    /// </summary>
    public class AuthEmailDomainRules
    {
        public AuthEmailDomainRules(
            bool allowsAnyDomain,
            IEnumerable<string> allowedDomains,
            IEnumerable<string> blockedDomains,
            IEnumerable<string> blockedDomainPrefixes)
        {
            AllowsAnyDomain = allowsAnyDomain;
            AllowedDomains = new HashSet<string>(allowedDomains);
            BlockedDomains = new HashSet<string>(blockedDomains);
            BlockedDomainPrefixes = new HashSet<string>(blockedDomainPrefixes);
        }

        /// <summary>
        /// When true, <see cref="AllowedDomains"/> is ignored. Blocked domains still apply.
        /// </summary>
        public bool AllowsAnyDomain { get; }

        /// <summary>
        /// e.g. "hackney.gov.uk". Subdomains are not included.
        /// </summary>
        public IReadOnlySet<string> AllowedDomains { get; }

        /// <summary>
        /// e.g. "prbly.win". Subdomains are not included.
        /// </summary>
        public IReadOnlySet<string> BlockedDomains { get; }

        /// <summary>
        /// e.g. "prbly" blocks "prbly.win" and "prblywin.com", but not "prbly" itself.
        /// </summary>
        public IReadOnlySet<string> BlockedDomainPrefixes { get; }
    }
}
