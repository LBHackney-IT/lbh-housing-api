namespace HousingRegisterApi.V1.Services
{
    public interface IAuthEmailDomainService
    {
        /// <summary>
        /// Whether the email's domain is allowed and not blocked.
        /// </summary>
        /// <param name="email">A validated, normalized email address</param>
        bool CanRequestVerificationCode(string email);
    }
}
