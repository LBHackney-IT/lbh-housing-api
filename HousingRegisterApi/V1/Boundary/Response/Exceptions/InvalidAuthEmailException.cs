namespace HousingRegisterApi.V1.Boundary.Response.Exceptions
{
    public class InvalidAuthEmailException : HousingRegisterException
    {
        public InvalidAuthEmailException()
            : base("Email address is not valid.")
        {
        }
    }
}
