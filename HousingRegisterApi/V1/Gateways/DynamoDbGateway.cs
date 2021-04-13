using Amazon.DynamoDBv2.DataModel;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Gateways
{
    public class DynamoDbGateway : IExampleGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }

        public IEnumerable<Entity> GetAll()
        {
            var conditions = new List<ScanCondition>();
            var search = _dynamoDbContext.ScanAsync<DatabaseEntity>(conditions).GetNextSetAsync().GetAwaiter().GetResult();
            return search.Select(x => x.ToDomain());
        }

        public Entity GetEntityById(int id)
        {
            var result = _dynamoDbContext.LoadAsync<DatabaseEntity>(id).GetAwaiter().GetResult();
            return result?.ToDomain();
        }
    }
}
