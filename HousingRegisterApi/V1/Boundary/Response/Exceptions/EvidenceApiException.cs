using System;

namespace HousingRegisterApi.V1.Boundary.Response.Exceptions
{
    public class EvidenceApiException : Exception
    {
        public EvidenceApiException(string message) : base(message)
        {
        }
    }
}
