using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Infrastructure
{
    public interface IDynamoDBSearchHelper
    {
        List<ScanCondition> GetScanConditions(string searchTerm);
    }
}
