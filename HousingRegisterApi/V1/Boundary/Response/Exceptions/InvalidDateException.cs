namespace HousingRegisterApi.V1.Boundary.Response.Exceptions
{
    public class InvalidDateException : HousingRegisterException
    {
        public InvalidDateException(string message) : base(message)
        {
        }
    }
}
