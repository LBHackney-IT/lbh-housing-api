using System.Collections.Generic;

namespace HousingRegisterApi.V1.Domain
{
    public class Applicant
    {
        public Person Person { get; set; }

        public Address Address { get; set; }
        public ContactInformation ContactInformation { get; set; }
        public IEnumerable<Question> Eligibility { get; set; }
        public IEnumerable<Question> Questions { get; set; }

        // TODO: other connecting information for the application, e.g. evidence?
    }
}
