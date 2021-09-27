using System.Collections.Generic;

namespace HousingRegisterApi.V1.Domain
{
    public class Applicant
    {
        public Person Person { get; set; }
        public Address Address { get; set; }
        public ContactInformation ContactInformation { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public MedicalNeed MedicalNeed { get; set; }
        public MedicalOutcome MedicalOutcome { get; set; }
    }
}
