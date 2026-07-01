using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HousingRegisterApi.V1.Infrastructure
{
    public static class AuthEmailValidator
    {
        private const int MaxEmailLength = 254;

        private static readonly char[] DisallowedCharacters =
        {
            '\'', '"', ';', '<', '>', '(', ')', '\\', '\r', '\n', '\0', ' '
        };

        public static bool IsValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var trimmed = email.Trim();

            if (trimmed.Length > MaxEmailLength)
            {
                return false;
            }

            if (trimmed.IndexOfAny(DisallowedCharacters) >= 0)
            {
                return false;
            }

            if (trimmed.Count(c => c == '@') != 1)
            {
                return false;
            }

            return new EmailAddressAttribute().IsValid(trimmed);
        }
    }
}
