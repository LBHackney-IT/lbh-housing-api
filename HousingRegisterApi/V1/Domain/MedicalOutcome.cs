using System;

namespace HousingRegisterApi.V1.Domain
{
    public class MedicalOutcome
    {
        public string AccessibileHousingRegister { get; set; }
        public string AdditionalInformaton { get; set; }
        public DateTime AssessmentDate { get; set; }
        public string Disability { get; set; }
        public string Outcome { get; set; }
    }
}
