using HousingRegisterApi.V1.Domain;
using System;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Boundary.Response
{
    //Guidance on XML comments and response objects here (https://github.com/LBHackney-IT/lbh-base-api/wiki/Controllers-and-Response-Objects)
    public class ApplicationResponse
    {
        public Guid Id { get; set; }

        public string Reference { get; set; }

        /// <example>Pending</example>
        public string Status { get; set; }

        /// <example>test@hackney.gov.uk</example>
        public string AssignedTo { get; set; }

        /// <example>2021-04-01</example>
        public DateTime CreatedAt { get; set; }

        /// <example>2021-04-01</example>
        public DateTime? SubmittedAt { get; set; }

        public Applicant MainApplicant { get; set; }

        public IEnumerable<Applicant> OtherMembers { get; set; }
    }
}
