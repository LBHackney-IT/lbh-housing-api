using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Gateways
{
    public class DynamoDbGateway : IApplicationApiGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ISHA256Helper _hashHelper;
        private readonly IVerifyCodeGenerator _codeGenerator;

        public DynamoDbGateway(
            IDynamoDBContext dynamoDbContext,
            ISHA256Helper hashHelper,
            IVerifyCodeGenerator codeGenerator)
        {
            _dynamoDbContext = dynamoDbContext;
            _hashHelper = hashHelper;
            _codeGenerator = codeGenerator;
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

        public Application GetApplicationById(Guid id)
        {
            var result = _dynamoDbContext.LoadAsync<ApplicationDbEntity>(id).GetAwaiter().GetResult();
            return result?.ToDomain();
        }

        public Application CreateNewApplication(CreateApplicationRequest request)
        {
            var entity = new ApplicationDbEntity
            {
                Id = Guid.NewGuid(),
                Reference = _hashHelper.Generate(request.MainApplicant.ContactInformation.EmailAddress).Substring(0, 10),
                CreatedAt = DateTime.UtcNow,
                SensitiveData = request.SensitiveData,
                SubmittedAt = null,
                Status = string.IsNullOrEmpty(request.Status) ? "New" : request.Status,
                MainApplicant = request.MainApplicant,
                OtherMembers = request.OtherMembers.ToList()
            };

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
                entity.Assessment = request.Assessment;

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
            entity.Status = "Submitted";

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

        public Application ConfirmVerifyCode(Guid id, VerifyAuthRequest request)
        {
            var entity = _dynamoDbContext.LoadAsync<ApplicationDbEntity>(id).GetAwaiter().GetResult();
            if (entity == null
                || entity.VerifyCode != request.Code
                || entity.VerifyExpiresAt < DateTime.UtcNow
                || entity.MainApplicant.ContactInformation.EmailAddress != request.Email)
            {
                return null;
            }

            return entity.ToDomain();
        }
    }
}
