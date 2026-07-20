namespace HousingRegisterApi.V1.Boundary.Response.Exceptions
{
    public class UnauthorizedStaffException : HousingRegisterException
    {
        public UnauthorizedStaffException()
            : base("Staff authorization is required to create an application.")
        {
        }
    }
}
