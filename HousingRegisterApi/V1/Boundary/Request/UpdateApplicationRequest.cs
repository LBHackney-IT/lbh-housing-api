using HousingRegisterApi.V1.Domain;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Boundary.Request
{
    public class UpdateApplicationRequest
    {
        public string Status { get; set; }
        public Person Applicant { get; set; }
        public IEnumerable<Person> OtherMembers { get; set; }
    }
}
