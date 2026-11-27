using System;

namespace HousingRegisterApi.V1.Boundary.Response.Exceptions
{
    public class EvidenceRequestNotAllowedException : Exception
    {
        public EvidenceRequestNotAllowedException(string status)
            : base($"Evidence can only be requested for a submitted application. Current status: {status}")
        {
        }
    }
}
