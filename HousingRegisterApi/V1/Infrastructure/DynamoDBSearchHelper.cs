using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class DynamoDBSearchHelper : IDynamoDBSearchHelper
    {
        public List<ScanCondition> Execute(string searchTerm)
        {
            var conditions = new List<ScanCondition>
            {
                new ScanCondition("MainApplicant.Person.Surname", ScanOperator.Equal, searchTerm),
            };

            var referenceNumberRegEx = @"^[a-z,0-9]{10,10}$";
            var isReferenceNumber = Regex.Match(searchTerm, referenceNumberRegEx, RegexOptions.IgnoreCase).Success;

            if (isReferenceNumber)
            {
                conditions.Clear();
                conditions.Add(new ScanCondition("Reference", ScanOperator.Equal, searchTerm));
                return conditions;
            }

            var nationalInsuranceRegEx = @"^(?!BG|GB|NK|KN|TN|NT|ZZ)[A-CEGHJ-PR-TW-Z][A-CEGHJ-NPR-TW-Z](?:\s*\d{2}){3}\s*[A-D]$";
            var isNationalInsuranceNuber = Regex.Match(searchTerm, nationalInsuranceRegEx, RegexOptions.IgnoreCase).Success;

            if (isNationalInsuranceNuber)
            {
                conditions.Clear();
                conditions.Add(new ScanCondition("MainApplicant.Person.NationalInsuranceNumber", ScanOperator.Equal, searchTerm));
                return conditions;
            }

            bool isValidGuid = Guid.TryParse(searchTerm, out Guid guidOutput);

            if (isValidGuid)
            {
                conditions.Clear();
                conditions.Add(new ScanCondition("Id", ScanOperator.Equal, searchTerm));
                return conditions;
            }

            return conditions;
        }
    }
}
