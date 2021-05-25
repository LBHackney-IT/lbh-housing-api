using Amazon.DynamoDBv2.DataModel;
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

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }

        public IEnumerable<Application> GetAll()
        {
            var conditions = new List<ScanCondition>();
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
                CreatedAt = DateTime.UtcNow,
                Status = request.Status,
                Applicant = request.Applicant,
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
            
            if (request.Applicant != null)
                entity.Applicant = request.Applicant;

            if (request.OtherMembers != null)
                entity.OtherMembers = request.OtherMembers.ToList();

            _dynamoDbContext.SaveAsync(entity).GetAwaiter().GetResult();
            return entity.ToDomain();
        }
    }
}
