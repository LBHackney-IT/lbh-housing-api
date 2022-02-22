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
        public Guid Id { get; set; }

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

        public string VerifyCode { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime? VerifyExpiresAt { get; set; }

        public string AssignedTo { get; set; } = "unassigned";

        public bool SensitiveData { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<Assessment>))]
        public Assessment Assessment { get; set; }

        public int? CalculatedBedroomNeed { get; set; }

        public bool ImportedFromLegacyDatabase { get; set; }

        protected DateTime SortDate {
            get => (SubmittedAt ?? CreatedAt);
        }

        public string SortKey
        {
            get => DynamoDbDateTimeConverter.FormatDate(SortDate) + ":" + Id;
            set { _ = value; }
        }

        public string StatusAssigneeKey
        {
            get => Status + ":" + AssignedTo;
            set { _ = value; }
        }

        public int ActiveRecord { get; set; } = 1;
    }
}
