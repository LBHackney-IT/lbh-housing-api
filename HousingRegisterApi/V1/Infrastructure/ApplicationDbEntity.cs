using Amazon.DynamoDBv2.DataModel;
using HousingRegisterApi.V1.Domain;
using System;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Infrastructure
{
    [DynamoDBTable("HousingRegister", LowerCamelCaseProperties = true)]
    public class ApplicationDbEntity
    {
        [DynamoDBHashKey]
        [DynamoDBProperty(AttributeName = "id")]
        public Guid Id { get; set; }

        [DynamoDBProperty(AttributeName = "reference")]
        public string Reference { get; set; }

        public string Status { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime? SubmittedAt { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<Applicant>))]
        public Applicant MainApplicant { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectListConverter<Applicant>))]
        public List<Applicant> OtherMembers { get; set; } = new List<Applicant>();
    }
}
