using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using HousingRegisterApi.V1.Domain.Sns;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace HousingRegisterApi.Tests
{
    public class DynamoDbIntegrationTests<TStartup> : IDisposable where TStartup : class
    {
        public HttpClient Client { get; private set; }
        public IDynamoDBContext DynamoDbContext => _factory?.DynamoDbContext;
        public SnsEventVerifier<ApplicationSns> SnsVerifer => _factory?.SnsVerifer;

        private readonly DynamoDbMockWebApplicationFactory<TStartup> _factory;
        private readonly List<TableDef> _tables = new List<TableDef>
        {
            new TableDef
            {
                Name = "HousingRegister",
                KeyName = "id",
                KeyType = ScalarAttributeType.S
            }
        };

        public DynamoDbIntegrationTests()
        {
            EnsureEnvVarConfigured("DynamoDb_LocalMode", "true");
            EnsureEnvVarConfigured("DynamoDb_LocalServiceUrl", "http://localhost:8000");
            EnsureEnvVarConfigured("Localstack_SnsServiceUrl", "http://localhost:4566");

            _factory = new DynamoDbMockWebApplicationFactory<TStartup>(_tables);
            Client = _factory.CreateClient();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (null != _factory)
                    _factory.Dispose();
                _disposed = true;
            }
        }

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }
    }

    public class TableDef
    {
        public string Name { get; set; }
        public string KeyName { get; set; }
        public ScalarAttributeType KeyType { get; set; }
    }
}
