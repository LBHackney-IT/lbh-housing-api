using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Hackney.Core.DynamoDb;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Gateways
{
    public class DynamoDbGateway : IApplicationApiGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly ISHA256Helper _hashHelper;
        private readonly IVerifyCodeGenerator _codeGenerator;
        private readonly IBedroomCalculatorService _bedroomCalculatorService;
        private readonly ILogger<DynamoDbGateway> _logger;

        public DynamoDbGateway(
            IDynamoDBContext dynamoDbContext,
            IAmazonDynamoDB dynamoDbClient,
            ISHA256Helper hashHelper,
            IVerifyCodeGenerator codeGenerator,
            IBedroomCalculatorService bedroomCalculatorService,
            ILogger<DynamoDbGateway> logger)
        {
            _dynamoDbContext = dynamoDbContext;
            _dynamoDbClient = dynamoDbClient;
            _hashHelper = hashHelper;
            _codeGenerator = codeGenerator;
            _bedroomCalculatorService = bedroomCalculatorService;
            _logger = logger;
        }

        public async Task<(IEnumerable<Application>, string)> GetApplicationsByStatusAsync(SearchQueryParameter searchParameters)
        {
            int pageSize = searchParameters.PageSize;
            var dbApplications = new List<ApplicationDbEntity>();
            var table = _dynamoDbContext.GetTargetTable<ApplicationDbEntity>();

            var queryConfig = new QueryOperationConfig
            {
                BackwardSearch = true,
                Limit = pageSize,
                PaginationToken = PaginationDetails.DecodeToken(searchParameters.PaginationToken),
                IndexName = "HousingRegisterStatus",
                KeyExpression = new Expression
                {
                    ExpressionStatement = "#status = :v_status",
                    ExpressionAttributeNames = {
                        { "#status", "status" }
                    },
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>()
                    {
                        { ":v_status", new Primitive(searchParameters.Status) },
                    },
                },
            };

            var search = table.Query(queryConfig);
            var resultsSet = await search.GetNextSetAsync().ConfigureAwait(false);

            var paginationToken = search.PaginationToken;
            if (resultsSet.Any())
            {
                dbApplications.AddRange(_dynamoDbContext.FromDocuments<ApplicationDbEntity>(resultsSet));
            }

            return (dbApplications.Select(x => x.ToDomain()), PaginationDetails.EncodeToken(paginationToken));
        }

        public async Task<(IEnumerable<Application>, string)> GetApplicationsByAssignedToAsync(SearchQueryParameter searchParameters)
        {
            int pageSize = searchParameters.PageSize;
            var dbApplications = new List<ApplicationDbEntity>();
            var table = _dynamoDbContext.GetTargetTable<ApplicationDbEntity>();

            var queryConfig = new QueryOperationConfig
            {
                BackwardSearch = true,
                Limit = pageSize,
                PaginationToken = PaginationDetails.DecodeToken(searchParameters.PaginationToken),
                IndexName = "HousingRegisterStatusAssignedTo",
                KeyExpression = new Expression
                {
                    ExpressionStatement = "statusAssigneeKey = :v_assignedTo",
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>()
                    {
                        { ":v_assignedTo", new Primitive(searchParameters.Status+":"+searchParameters.AssignedTo) },
                    },
                },
            };

            var search = table.Query(queryConfig);
            var resultsSet = await search.GetNextSetAsync().ConfigureAwait(false);

            var paginationToken = search.PaginationToken;
            if (resultsSet.Any())
            {
                dbApplications.AddRange(_dynamoDbContext.FromDocuments<ApplicationDbEntity>(resultsSet));
            }

            return (dbApplications.Select(x => x.ToDomain()), PaginationDetails.EncodeToken(paginationToken));
        }

        public async Task<(IEnumerable<Application>, string)> GetApplicationsByReferenceAsync(SearchQueryParameter searchParameters)
        {
            int pageSize = searchParameters.PageSize;
            var dbApplications = new List<ApplicationDbEntity>();
            var table = _dynamoDbContext.GetTargetTable<ApplicationDbEntity>();

            var queryConfig = new QueryOperationConfig
            {
                BackwardSearch = true,
                Limit = pageSize,
                PaginationToken = PaginationDetails.DecodeToken(searchParameters.PaginationToken),
                IndexName = "HousingRegisterReference",
                KeyExpression = new Expression
                {
                    ExpressionStatement = "#reference = :v_reference",
                    ExpressionAttributeNames = {
                        { "#reference", "reference" }
                    },
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>()
                    {
                        { ":v_reference", new Primitive(searchParameters.Reference) },
                    },
                },
            };

            var search = table.Query(queryConfig);
            var resultsSet = await search.GetNextSetAsync().ConfigureAwait(false);

            var paginationToken = search.PaginationToken;
            if (resultsSet.Any())
            {
                dbApplications.AddRange(_dynamoDbContext.FromDocuments<ApplicationDbEntity>(resultsSet));
            }

            return (dbApplications.Select(x => x.ToDomain()), PaginationDetails.EncodeToken(paginationToken));
        }

        public async Task<(IEnumerable<Application>, string)> GetAllApplicationsAsync(SearchQueryParameter searchParameters)
        {
            int pageSize = searchParameters.PageSize;
            var dbApplications = new List<ApplicationDbEntity>();
            var table = _dynamoDbContext.GetTargetTable<ApplicationDbEntity>();

            var queryConfig = new QueryOperationConfig
            {
                BackwardSearch = true,
                Limit = pageSize,
                PaginationToken = PaginationDetails.DecodeToken(searchParameters.PaginationToken),
                IndexName = "HousingRegisterAll",
                KeyExpression = new Expression
                {
                    ExpressionStatement = "activeRecord = :v_activeRecord",
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>()
                    {
                        { ":v_activeRecord", new Primitive("1", true) },
                    },
                },
            };

            var search = table.Query(queryConfig);
            var resultsSet = await search.GetNextSetAsync().ConfigureAwait(false);

            var paginationToken = search.PaginationToken;
            if (resultsSet.Any())
            {
                dbApplications.AddRange(_dynamoDbContext.FromDocuments<ApplicationDbEntity>(resultsSet));
            }

            return (dbApplications.Select(x => x.ToDomain()), PaginationDetails.EncodeToken(paginationToken));
        }

        public async Task<(IEnumerable<Application>, string)> GetApplicationsAsync(SearchQueryParameter searchParameters)
        {
            int pageSize = searchParameters.PageSize;
            var dbApplications = new List<ApplicationDbEntity>();
            var table = _dynamoDbContext.GetTargetTable<ApplicationDbEntity>();

            var scanConfig = new ScanOperationConfig()
            {
                Limit = pageSize,
                PaginationToken = PaginationDetails.DecodeToken(searchParameters.PaginationToken)
            };

            if (!string.IsNullOrEmpty(searchParameters.Status))
            {
                scanConfig.Filter.AddCondition(nameof(ApplicationDbEntity.Status).ToCamelCase(), ScanOperator.Equal, searchParameters.Status);
            }
            if (!string.IsNullOrEmpty(searchParameters.Reference))
            {
                scanConfig.Filter.AddCondition(nameof(ApplicationDbEntity.Reference).ToCamelCase(), ScanOperator.Contains, searchParameters.Reference);
            }
            if (!string.IsNullOrEmpty(searchParameters.AssignedTo))
            {
                if (searchParameters.AssignedTo == "unassigned")
                {
                    scanConfig.Filter.AddCondition(nameof(ApplicationDbEntity.AssignedTo).ToCamelCase(), ScanOperator.IsNull);
                }
                else
                {
                    scanConfig.Filter.AddCondition(nameof(ApplicationDbEntity.AssignedTo).ToCamelCase(), ScanOperator.Equal, searchParameters.AssignedTo);
                }

            }
            if (searchParameters.HasAssessment.Value)
            {
                scanConfig.Filter.AddCondition(nameof(Assessment).ToCamelCase(), ScanOperator.IsNotNull);
            }

            var search = table.Scan(scanConfig);
            var resultsSet = await search.GetNextSetAsync().ConfigureAwait(false);

            var paginationToken = search.PaginationToken;

            if (resultsSet.Any())
            {
                dbApplications.AddRange(_dynamoDbContext.FromDocuments<ApplicationDbEntity>(resultsSet));
            }

            return (dbApplications.Select(x => x.ToDomain()), PaginationDetails.EncodeToken(paginationToken));
        }

        public IEnumerable<Application> GetApplications(SearchQueryParameter searchParameters)
        {
            // scan conditions
            var conditions = new List<ScanCondition>();
            if (!string.IsNullOrEmpty(searchParameters.Status))
            {
                conditions.Add(new ScanCondition(nameof(ApplicationDbEntity.Status), ScanOperator.Equal, searchParameters.Status));
            }
            if (!string.IsNullOrEmpty(searchParameters.Reference))
            {
                conditions.Add(new ScanCondition(nameof(ApplicationDbEntity.Reference), ScanOperator.Contains, searchParameters.Reference));
            }
            if (!string.IsNullOrEmpty(searchParameters.AssignedTo))
            {
                var assignCondition = searchParameters.AssignedTo == "unassigned"
                    ? new ScanCondition(nameof(ApplicationDbEntity.AssignedTo), ScanOperator.IsNull)
                    : new ScanCondition(nameof(ApplicationDbEntity.AssignedTo), ScanOperator.Equal, searchParameters.AssignedTo);

                conditions.Add(assignCondition);
            }
            if (searchParameters.HasAssessment.HasValue)
            {
                if (searchParameters.HasAssessment.Value)
                {
                    conditions.Add(new ScanCondition(nameof(Assessment), ScanOperator.IsNotNull));
                }
            }

            // query dynamodb
            var search = _dynamoDbContext.ScanAsync<ApplicationDbEntity>(conditions).GetNextSetAsync().GetAwaiter().GetResult();
            var searchItems = search.Select(x => x.ToDomain());

            // filter on nested values
            if (!string.IsNullOrEmpty(searchParameters.Surname))
            {
                return searchItems.Where(x => x.MainApplicant.Person.Surname.Contains(searchParameters.Surname, StringComparison.InvariantCultureIgnoreCase));
            }
            if (!string.IsNullOrEmpty(searchParameters.NationalInsurance))
            {
                return searchItems.Where(x => x.MainApplicant.Person.NationalInsuranceNumber.Contains(searchParameters.NationalInsurance, StringComparison.InvariantCultureIgnoreCase));
            }

            return searchItems;
        }

        public IEnumerable<Application> GetApplicationsAtStatus(params string[] status)
        {
            var statusNames = new List<string>();
            var statusNameAndValues = new Dictionary<string, DynamoDBEntry>();

            for (int i = 0; i < status.Length; i++)
            {
                string searchName = $":status{i}";
                statusNames.Add(searchName);
                statusNameAndValues.Add(searchName, new Primitive(status[i]));
            }

            // status is a reserved word, so we have to map it to something else, ie. #application_status
            var scanConfig = new ScanOperationConfig
            {
                FilterExpression = new Expression()
                {
                    ExpressionStatement = $"#application_status IN ({string.Join(",", statusNames.ToArray())})",
                    ExpressionAttributeValues = statusNameAndValues,
                    ExpressionAttributeNames = new Dictionary<string, string>()
                    {
                        {"#application_status", "status" }
                    }
                },
            };

            var search = _dynamoDbContext.FromScanAsync<ApplicationDbEntity>(scanConfig).GetRemainingAsync().GetAwaiter().GetResult();
            var searchItems = search.Select(x => x.ToDomain());

            return searchItems;
        }

        public IEnumerable<Application> GetApplicationsAtStatusForNonLegacy(params string[] status)
        {
            var statusNames = new List<string>();
            var statusNameAndValues = new Dictionary<string, DynamoDBEntry>();

            for (int i = 0; i < status.Length; i++)
            {
                string searchName = $":status{i}";
                statusNames.Add(searchName);
                statusNameAndValues.Add(searchName, new Primitive(status[i]));
            }
            statusNameAndValues.Add(":v_legacyDbFalse", new Primitive("0", true));
            // status is a reserved word, so we have to map it to something else, ie. #application_status
            // Here we are assuming that if the importedFromLegacyDatabase attribute is missing than it does not come from the legacy database
            var scanConfig = new ScanOperationConfig
            {
                FilterExpression = new Expression()
                {
                    ExpressionStatement = $"#application_status IN ({string.Join(",", statusNames.ToArray())}) " +
                        $"AND (importedFromLegacyDatabase = :v_legacyDbFalse OR attribute_not_exists(importedFromLegacyDatabase))",
                    ExpressionAttributeValues = statusNameAndValues,
                    ExpressionAttributeNames = new Dictionary<string, string>()
                {
                    {"#application_status", "status" }
                }
                },
            };


            var search = _dynamoDbContext.FromScanAsync<ApplicationDbEntity>(scanConfig).GetRemainingAsync().GetAwaiter().GetResult();
            var searchItems = search.Select(x => x.ToDomain());

            return searchItems;
        }

        public Application GetApplicationById(Guid id)
        {
            var result = _dynamoDbContext.LoadAsync<ApplicationDbEntity>(id).GetAwaiter().GetResult();
            return result?.ToDomain();
        }

        public Application GetIncompleteApplication(string email)
        {
            string reference = _hashHelper.Generate(email).Substring(0, 10);

            var dbApplications = new List<ApplicationDbEntity>();
            var table = _dynamoDbContext.GetTargetTable<ApplicationDbEntity>();

            var queryConfig = new QueryOperationConfig
            {
                IndexName = "HousingRegisterReference",
                KeyExpression = new Expression
                {
                    ExpressionStatement = "#reference = :v_reference",
                    ExpressionAttributeNames = {
                        { "#reference", "reference" }
                    },
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>()
                    {
                        { ":v_reference", new Primitive(reference) },
                    },
                },
            };

            var search = table.Query(queryConfig);
            var resultsSet = search.GetNextSetAsync().GetAwaiter().GetResult();

            return _dynamoDbContext
                .FromDocuments<ApplicationDbEntity>(resultsSet)
                .Select(x => x.ToDomain())
                .FirstOrDefault();
        }

        public Application CreateNewApplication(CreateApplicationRequest request)
        {
            var newApplicationGuid = Guid.NewGuid();

            string emailAddress = request.MainApplicant?.ContactInformation?.EmailAddress;

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                emailAddress = null;
            }

            var entity = new ApplicationDbEntity
            {
                Id = newApplicationGuid,
                Reference = _hashHelper.Generate(emailAddress ?? newApplicationGuid.ToString()).Substring(0, 10),
                CreatedAt = DateTime.UtcNow,
                SensitiveData = request.SensitiveData,
                SubmittedAt = null,
                Status = string.IsNullOrEmpty(request.Status) ? ApplicationStatus.New : request.Status,
                MainApplicant = request.MainApplicant,
                OtherMembers = request.OtherMembers.ToList(),
                ImportedFromLegacyDatabase = false
            };

            entity.CalculatedBedroomNeed = _bedroomCalculatorService.Calculate(entity.ToDomain());

            _dynamoDbContext.SaveAsync(entity).GetAwaiter().GetResult();
            return entity.ToDomain();
        }

        public Application UpdateApplication(Guid id, UpdateApplicationRequest request)
        {
            var entity = _dynamoDbContext.LoadAsync<ApplicationDbEntity>(id).GetAwaiter().GetResult();
            if (entity == null)
            {
                return null;
            }

            if (request.SensitiveData.HasValue)
                entity.SensitiveData = request.SensitiveData.Value;

            if (!string.IsNullOrEmpty(request.Status))
                entity.Status = request.Status;

            if (!string.IsNullOrEmpty(request.AssignedTo))
                entity.AssignedTo = request.AssignedTo;

            if (request.MainApplicant != null)
                entity.MainApplicant = request.MainApplicant;

            if (request.OtherMembers != null)
                entity.OtherMembers = request.OtherMembers.ToList();

            if (request.Assessment != null)
                entity.Assessment = request.Assessment.ToDomain();

            entity.CalculatedBedroomNeed = _bedroomCalculatorService.Calculate(entity.ToDomain());

            _dynamoDbContext.SaveAsync(entity).GetAwaiter().GetResult();

            return entity.ToDomain();
        }

        public Application CompleteApplication(Guid id)
        {
            var entity = _dynamoDbContext.LoadAsync<ApplicationDbEntity>(id).GetAwaiter().GetResult();
            if (entity?.MainApplicant == null)
            {
                return null;
            }

            entity.SubmittedAt = DateTime.UtcNow;
            entity.Status = ApplicationStatus.Submitted;

            _dynamoDbContext.SaveAsync(entity).GetAwaiter().GetResult();

            return entity.ToDomain();
        }

        public Application CreateVerifyCode(Guid id, CreateAuthRequest request)
        {
            var entity = _dynamoDbContext.LoadAsync<ApplicationDbEntity>(id).GetAwaiter().GetResult();
            if (entity == null
                || entity.MainApplicant.ContactInformation.EmailAddress != request.Email)
            {
                return null;
            }

            entity.VerifyCode = _codeGenerator.GenerateCode();
            entity.VerifyExpiresAt = DateTime.UtcNow.AddMinutes(30);

            _dynamoDbContext.SaveAsync(entity).GetAwaiter().GetResult();

            return entity.ToDomain();
        }

        /// <summary>
        /// Verifies that an application exists for the specified email and verification code
        /// </summary>
        /// <param name="request"></param>
        /// <returns>An application or null</returns>
        public Application ConfirmVerifyCode(VerifyAuthRequest request)
        {
            var application = GetIncompleteApplication(request.Email);

            if (application == null
                || application.VerifyCode != request.Code
                || application.VerifyExpiresAt < DateTime.UtcNow)
            {
                return null;
            }

            // if code has been verified, nullify the fields so they can't be used again
            var entity = _dynamoDbContext.LoadAsync<ApplicationDbEntity>(application.Id).GetAwaiter().GetResult();
            entity.VerifyCode = null;
            entity.VerifyExpiresAt = null;

            // progress this application to new if its at verfication
            if (entity.Status == ApplicationStatus.Verification)
            {
                entity.Status = ApplicationStatus.New;
            }

            _dynamoDbContext.SaveAsync(entity).GetAwaiter().GetResult();

            return entity.ToDomain();
        }

        public Application ImportApplication(ImportApplicationRequest request)
        {
            var entity = new ApplicationDbEntity
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                SubmittedAt = request.SubmittedAt,
                MainApplicant = request.MainApplicant,
                Status = string.IsNullOrEmpty(request.Status) ? ApplicationStatus.New : request.Status,
                Assessment = request.Assessment,
                ImportedFromLegacyDatabase = true
            };

            _dynamoDbContext.SaveAsync(entity).GetAwaiter().GetResult();
            return entity.ToDomain();
        }

        public async Task<long> IssueNextBiddingNumber()
        {
            string tableName = "HousingRegister";
            long nextBiddingNumber = 0;

            try
            {
                var request = new UpdateItemRequest
                {
                    Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue { S = "HousingRegister#BiddingNumberAtomicCounter" } } },
                    ExpressionAttributeNames = new Dictionary<string, string>()
    {
        {"#B", "lastIssuedBiddingNumber"}
    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
    {
        {":incr",new AttributeValue {N = "1"}}
    },
                    UpdateExpression = "SET #B = #B + :incr",
                    TableName = tableName,
                    ReturnValues = ReturnValue.UPDATED_NEW
                };

                var response = await _dynamoDbClient.UpdateItemAsync(request).ConfigureAwait(false);

                string newBiddingNumber = response.Attributes["lastIssuedBiddingNumber"].N;

                nextBiddingNumber = long.Parse(newBiddingNumber);
            }
            catch (AmazonDynamoDBException ex)
            {
                _logger.LogError(ex, "Error getting next bidding number transactionally");
                throw;
            }

            return nextBiddingNumber;

        }

        public async Task<long?> GetLastIssuedBiddingNumber()
        {
            string tableName = "HousingRegister";
            long? nextBiddingNumber = null;

            try
            {
                var request = new GetItemRequest
                {
                    Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue { S = "HousingRegister#BiddingNumberAtomicCounter" } } },
                    TableName = tableName
                };

                var response = await _dynamoDbClient.GetItemAsync(request).ConfigureAwait(false);

                string newBiddingNumber = response.Item["lastIssuedBiddingNumber"].S;

                nextBiddingNumber = long.Parse(newBiddingNumber);
            }
            catch (AmazonDynamoDBException ex)
            {
                _logger.LogError(ex, "Error getting next bidding number transactionally");
            }

            return nextBiddingNumber;

        }
    }
}
