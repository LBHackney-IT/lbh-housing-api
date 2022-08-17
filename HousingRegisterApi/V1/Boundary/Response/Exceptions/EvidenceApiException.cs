using System;

namespace HousingRegisterApi.V1.Boundary.Response.Exceptions
{
    public class EvidenceApiException : HousingRegisterException
    {
        public EvidenceApiException(string message) : base(message)
        {
        }
    }
}
