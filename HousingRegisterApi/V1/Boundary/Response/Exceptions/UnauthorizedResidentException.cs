namespace HousingRegisterApi.V1.Boundary.Response.Exceptions
{
    public class UnauthorizedResidentException : HousingRegisterException
    {
        public UnauthorizedResidentException()
            : base("Resident authorization is required for this application.")
        {
        }
    }
}
