using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace HousingRegisterApi.V1.Domain
{
    public class Applicant
    {
        [DynamoDBProperty("Person")]
        public Person Person { get; set; }

        public Address Address { get; set; }
        public ContactInformation ContactInformation { get; set; }
        public IEnumerable<Question> Eligibility { get; set; }
        public IEnumerable<Question> Questions { get; set; }

        // TODO: other connecting information for the application, e.g. evidence?
    }
}
