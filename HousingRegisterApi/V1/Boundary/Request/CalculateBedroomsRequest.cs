using HousingRegisterApi.V1.Domain;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Boundary.Request
{
    public class CalculateBedroomsRequest
    {
        public Applicant MainApplicant { get; set; }
        public IEnumerable<Applicant> OtherMembers { get; set; }
    }
}
