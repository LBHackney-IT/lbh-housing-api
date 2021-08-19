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

            var referenceNumberRegEx = @".*\\d+.*";
            var isReferenceNumber = searchTerm.Length == 10 && Regex.Match(searchTerm, referenceNumberRegEx, RegexOptions.IgnoreCase).Success;

            if (isReferenceNumber)
            {
                conditions.Clear();
                conditions.Add(new ScanCondition("Reference", ScanOperator.Equal, searchTerm));
            }

            var nationalInsuranceRegEx = @"^\s*[a-zA-Z]{2}(?:\s*\d\s*){6}[a-zA-Z]?\s*$";

            var isNationalInsuranceNuber = Regex.Match(searchTerm, nationalInsuranceRegEx, RegexOptions.IgnoreCase).Success;

            if (isNationalInsuranceNuber)
            {
                conditions.Clear();
                conditions.Add(new ScanCondition("MainApplicant.Person.NationalInsuranceNumber", ScanOperator.Equal, searchTerm));
            }

            bool isValidGuid = Guid.TryParse(searchTerm, out Guid guidOutput);

            if (isValidGuid)
            {
                conditions.Clear();
                conditions.Add(new ScanCondition("Id", ScanOperator.Equal, guidOutput));
            }

            return conditions;
        }
    }
}
