using HousingRegisterApi.V1.Domain;
using System;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Boundary.Response
{
    //Guidance on XML comments and response objects here (https://github.com/LBHackney-IT/lbh-base-api/wiki/Controllers-and-Response-Objects)
    public class ApplicationResponse
    {                
        public Guid Id { get; set; }
        /// <example>Pending</example>
        public string Status { get; set; }
        /// <example>2021-04-12</example>
        public DateTime CreatedAt { get; set; }
        public Person Applicant { get; set; }
        public IEnumerable<Person> OtherMembers { get; set; }
    }
}
