using System;

namespace HousingRegisterApi.V1.Domain
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Firstname { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string Ethnicity { get; set; }
        public string Nationality { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public Address Address { get; set; }
        public ContactInformation ContactInformation { get; set; }
    }
}