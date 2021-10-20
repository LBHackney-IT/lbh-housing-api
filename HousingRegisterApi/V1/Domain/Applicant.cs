using System.Collections.Generic;

namespace HousingRegisterApi.V1.Domain
{
    public class Applicant
    {
        public Person Person { get; set; }
        public Address Address { get; set; }
        public ContactInformation ContactInformation { get; set; }
        public IEnumerable<Question> Questions { get; set; }

        public bool RequiresMedical => Questions.GetAnswer("medical-needs/medical-needs") == "yes";
        public MedicalNeed MedicalNeed { get; set; }
    }
}
