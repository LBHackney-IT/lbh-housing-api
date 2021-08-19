using Amazon.DynamoDBv2.DataModel;
using System;

namespace HousingRegisterApi.V1.Domain
{
    // TODO: integrate with the Person API
    public class Person
    {
        public Person()
        {
            if (Id == Guid.Empty)
            {
                Id = Guid.NewGuid();
            }
        }

        public Guid Id { get; set; }

        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        [DynamoDBProperty("Surname")]
        public string Surname { get; set; }

        public string PlaceOfBirth { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string PersonType { get; set; }
        public string RelationshipType { get; set; }

        public string Ethnicity { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }

        [DynamoDBProperty("NationalInsuranceNumber")]
        public string NationalInsuranceNumber { get; set; }
    }
}
