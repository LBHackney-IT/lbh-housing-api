namespace HousingRegisterApi.V1.Boundary.Response.Exceptions
{
    public class InvalidBiddingNumberException : HousingRegisterException
    {
        public InvalidBiddingNumberException(string message) : base(message)
        {
        }
    }
}
