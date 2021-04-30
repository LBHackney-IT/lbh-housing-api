using System;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Domain
{    
    public class Application
    {
        public Guid Id { get; set; }
        // TODO: should this be a type?
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Person Applicant { get; set; }
        public IEnumerable<Person> OtherMembers { get; set; }
    }
}
