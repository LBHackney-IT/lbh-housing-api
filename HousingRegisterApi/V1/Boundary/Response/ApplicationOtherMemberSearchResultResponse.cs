using System;

namespace HousingRegisterApi.V1.Boundary.Response
{
    public class ApplicationOtherMemberSearchResultResponse
    {
        public Guid Id { get; set; }
        public string NationalInsuranceNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public String FirstName { get; set; }

        public string MiddleName { get; set; }

        public string Surname { get; set; }
    }
}
