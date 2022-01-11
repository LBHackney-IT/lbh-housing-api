using HousingRegisterApi.V1.Domain;
using System;

namespace HousingRegisterApi.V1.Boundary.Request
{
    public class ImportApplicationRequest
    {
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; }
        public Applicant MainApplicant { get; set; }
        public Assessment Assessment { get; set; }
    }
}
