using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class FixDOBUseCase : IFixDOBUseCase
    {
        private readonly IAmazonDynamoDB _dynamo;
        private readonly ILogger<FixDOBUseCase> _logger;

        public FixDOBUseCase(IAmazonDynamoDB dynamo, ILogger<FixDOBUseCase> logger)
        {
            _dynamo = dynamo;
            _logger = logger;
        }

        public async Task Execute()
        {
            Table productCatalogTable = Table.LoadTable(_dynamo, "HousingRegister");

            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition("activeRecord", ScanOperator.Equal, 1);
            ScanOperationConfig config = new ScanOperationConfig()
            {
                Filter = scanFilter,
                Select = SelectValues.AllAttributes
            };

            Search search = productCatalogTable.Scan(config);

            List<Document> documentList;
            do
            {
                documentList = await search.GetNextSetAsync().ConfigureAwait(false);
                foreach (var application in documentList)
                {
                    //Check main applicants DOB
                    string DOB = application["mainApplicant"].AsDocument()["person"].AsDocument()["dateOfBirth"];
                    DateTimeOffset currentDOB = DateTimeOffset.Parse(DOB);

                    //Fix main applicants DOB
                    if (currentDOB.Hour == 23)
                    {
                        DateTimeOffset newDOB;
                        newDOB = currentDOB.AddHours(1);
                        await UpdateMainApplicantDob(application, currentDOB, newDOB).ConfigureAwait(false);
                    }

                    int index = 0;
                    foreach (var member in application["otherMembers"].AsListOfDocument())
                    {
                        DOB = member["person"].AsDocument()["dateOfBirth"];
                        currentDOB = DateTimeOffset.Parse(DOB);

                        if (currentDOB.Hour == 23)
                        {
                            DateTimeOffset newDOB;
                            newDOB = currentDOB.AddHours(1);
                            await UpdateOtherApplicantDob(application, index, currentDOB, newDOB).ConfigureAwait(false);
                        }
                        index++;
                    }
                }
            } while (!search.IsDone);
        }

        private async Task UpdateMainApplicantDob(Document application, DateTimeOffset currentDOB, DateTimeOffset newDOB)
        {
            var response = await _dynamo.UpdateItemAsync(new UpdateItemRequest
            {
                TableName = "HousingRegister",
                ExpressionAttributeNames = new Dictionary<string, string> {
                    { "#main", "mainApplicant" },
                    { "#per", "person" },
                    { "#dob", "dateOfBirth" },
                    { "#olddob", "oldDateOfBirth" },
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    { ":newDOB", new AttributeValue(s: newDOB.ToString("o", CultureInfo.InvariantCulture)) },
                    { ":oldDOB", new AttributeValue(s: currentDOB.ToString("o", CultureInfo.InvariantCulture))}
                },
                UpdateExpression = "SET #main.#per.#dob = :newDOB, #main.#per.#olddob = :oldDOB",
                Key = new Dictionary<string, AttributeValue> { { "id", new AttributeValue(s: application["id"]) } }
            }).ConfigureAwait(false);            
        }

        private async Task UpdateOtherApplicantDob(Document application, int otherMemberIndex, DateTimeOffset currentDOB, DateTimeOffset newDOB)
        {
            var response = await _dynamo.UpdateItemAsync(new UpdateItemRequest
            {
                TableName = "HousingRegister",
                ExpressionAttributeNames = new Dictionary<string, string> {
                    { "#oth", "otherMembers" },
                    { "#per", "person" },
                    { "#dob", "dateOfBirth" },
                    { "#olddob", "oldDateOfBirth" },
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    { ":newDOB", new AttributeValue(s: newDOB.ToString("o", CultureInfo.InvariantCulture)) },
                    { ":oldDOB", new AttributeValue(s: currentDOB.ToString("o", CultureInfo.InvariantCulture))}
                },
                UpdateExpression = $"SET #oth[{otherMemberIndex}].#per.#dob = :newDOB, #oth[{otherMemberIndex}].#per.#olddob = :oldDOB",
                ConditionExpression = $"#oth[{otherMemberIndex}].#per.#dob = :oldDOB",
                Key = new Dictionary<string, AttributeValue> { { "id", new AttributeValue(s: application["id"]) } }
            }).ConfigureAwait(false);
        }
    }
}
