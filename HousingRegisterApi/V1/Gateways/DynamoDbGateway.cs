using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HousingRegisterApi.V1.Gateways
{
    public class DynamoDbGateway : IApplicationApiGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ISHA256Helper _hashHelper;
        private readonly IDynamoDBSearchHelper _dynamoDBSearchHelper;

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext, ISHA256Helper hashHelper, IDynamoDBSearchHelper dynamoDBSearchHelper)
        {
            _dynamoDbContext = dynamoDbContext;
            _hashHelper = hashHelper;
            _dynamoDBSearchHelper = dynamoDBSearchHelper;
        }

        public IEnumerable<Application> GetAll()
        {
            var conditions = new List<ScanCondition>();
            var search = _dynamoDbContext.ScanAsync<ApplicationDbEntity>(conditions).GetNextSetAsync().GetAwaiter().GetResult();
            return search.Select(x => x.ToDomain());
        }

        public IEnumerable<Application> GetAllBySearchTerm(string searchTerm)
        {
            var conditions = _dynamoDBSearchHelper.Execute(searchTerm);
            var search = _dynamoDbContext.ScanAsync<ApplicationDbEntity>(conditions).GetNextSetAsync().GetAwaiter().GetResult();
            return search.Select(x => x.ToDomain());
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

            if (!string.IsNullOrEmpty(request.Status))
                entity.Status = request.Status;

            if (request.MainApplicant != null)
                entity.MainApplicant = request.MainApplicant;

            if (request.OtherMembers != null)
                entity.OtherMembers = request.OtherMembers.ToList();

            _dynamoDbContext.SaveAsync(entity).GetAwaiter().GetResult();

            return entity.ToDomain();
        }

        public Application CompleteApplication(Guid id)
        {
            var entity = _dynamoDbContext.LoadAsync<ApplicationDbEntity>(id).GetAwaiter().GetResult();
            if (entity == null)
            {
                return null;
            }

            if (entity.MainApplicant != null)
            {
                return null;
            }

            entity.SubmittedAt = DateTime.UtcNow;
            entity.Status = "Submitted";

            _dynamoDbContext.SaveAsync(entity).GetAwaiter().GetResult();

            return entity.ToDomain();
        }
    }
}
