using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.S3.Model;
using HousingRegisterApi.V1.Domain.Sns;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace HousingRegisterApi.Tests
{
    public class DynamoDbIntegrationTests<TStartup> : IDisposable where TStartup : class
    {
        private IAmazonS3 AmazonStorage => _factory?.AmazonS3;
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

        public async Task CreateTestFile(string fileName, string fileTag = null)
        {
            string bucket = Environment.GetEnvironmentVariable("HOUSINGREGISTER_EXPORT_BUCKET_NAME");

            byte[] fileData = Encoding.UTF8.GetBytes("test file");

            using MemoryStream ms = new MemoryStream(fileData);
            PutObjectRequest file = new PutObjectRequest()
            {
                BucketName = bucket,
                Key = fileName,
                InputStream = ms,
            };

            if (fileTag != null)
            {
                file.TagSet.Add(new Tag() { Key = "status", Value = fileTag });
            }

            await AmazonStorage.PutObjectAsync(file).ConfigureAwait(false);
        }

        public async Task<List<Tag>> GetFileTags(string fileName)
        {
            string bucket = Environment.GetEnvironmentVariable("HOUSINGREGISTER_EXPORT_BUCKET_NAME");

            GetObjectTaggingRequest file = new GetObjectTaggingRequest()
            {
                BucketName = bucket,
                Key = fileName,
            };

            var tag = await AmazonStorage.GetObjectTaggingAsync(file).ConfigureAwait(false);
            return tag.Tagging;
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
