using HousingRegisterApi.V1.Boundary.Response.Exceptions;

namespace HousingRegisterApi.V1.UseCase
{
    public class AuthGenerateBlockedException : HousingRegisterException
    {
        public AuthGenerateBlockedException()
            : base("Unable to generate a verification code for this email address.")
        {
        }
    }
}
