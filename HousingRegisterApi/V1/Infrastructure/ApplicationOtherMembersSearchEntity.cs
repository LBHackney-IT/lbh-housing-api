using System;
using System.Collections.Generic;
using System.Text;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class ApplicationOtherMembersSearchEntity
    {

        public Guid Id { get; set; }
        public string NationalInsuranceNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public String FirstName { get; set; }

        public string MiddleName { get; set; }

        public string Surname { get; set; }
    }
}
