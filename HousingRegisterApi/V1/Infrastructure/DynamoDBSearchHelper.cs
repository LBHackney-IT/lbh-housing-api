using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class DynamoDBSearchHelper : IDynamoDBSearchHelper
    {
        public List<ScanCondition> GetScanConditions(string searchTerm)
        {
            var conditions = new List<ScanCondition>();

            var referenceNumberRegEx = @"^[a-z,0-9]{10,10}$";
            var isReferenceNumber = Regex.Match(searchTerm, referenceNumberRegEx, RegexOptions.IgnoreCase).Success;
            if (isReferenceNumber)
            {
                conditions.Add(new ScanCondition("Reference", ScanOperator.Equal, searchTerm));
            }

            return conditions;
        }
    }
}
