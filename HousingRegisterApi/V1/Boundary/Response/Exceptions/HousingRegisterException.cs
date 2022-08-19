using System;
using System.Runtime.Serialization;

namespace HousingRegisterApi.V1.Boundary.Response.Exceptions
{
    public class HousingRegisterException : Exception
    {

        public HousingRegisterException(string message) : base(message)
        {
        }

    }
}
