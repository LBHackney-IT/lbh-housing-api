using System;

namespace HousingRegisterApi.V1.Boundary.Response.Exceptions
{
    public class DuplicateBiddingNumberException : Exception
    {
        public DuplicateBiddingNumberException(string message) : base(message)
        {
        }
    }
}
