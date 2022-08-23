using System;

namespace HousingRegisterApi.V1.Boundary.Response.Exceptions
{
    public class DuplicateBiddingNumberException : HousingRegisterException
    {
        public DuplicateBiddingNumberException(string message) : base(message)
        {
        }
    }
}
