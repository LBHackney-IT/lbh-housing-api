namespace HousingRegisterApi.V1.Infrastructure
{
    public static class EmailNormalizer
    {
        public static string Normalize(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            return email.Trim().ToLowerInvariant();
        }

        /// <summary>
        /// Compares two email addresses the way the application's login lookup should:
        /// case and surrounding whitespace are ignored. Two blank values never match.
        /// </summary>
        public static bool IsSameAddress(string stored, string requested)
        {
            var normalizedStored = Normalize(stored);
            return normalizedStored != null && normalizedStored == Normalize(requested);
        }
    }
}
