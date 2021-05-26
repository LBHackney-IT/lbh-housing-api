using HousingRegisterApi.V1.Domain;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Boundary.Request
{
    public class UpdateApplicationRequest
    {
        public string Status { get; set; }
        public Applicant MainApplicant { get; set; }
        public IEnumerable<Applicant> OtherMembers { get; set; }
    }
}
