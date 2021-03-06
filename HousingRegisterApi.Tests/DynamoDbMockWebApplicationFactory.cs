using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using HousingRegisterApi.V1.Domain.Sns;
using HousingRegisterApi.V1.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Notify.Interfaces;
using System;
using System.Collections.Generic;

namespace HousingRegisterApi.Tests
{
    public class DynamoDbMockWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly List<TableDef> _tables;

        public IAmazonDynamoDB DynamoDb { get; private set; }
        public IDynamoDBContext DynamoDbContext { get; private set; }
        public IAmazonSimpleNotificationService SimpleNotificationService { get; private set; }
        public IAmazonSQS AmazonSQS { get; private set; }
        public IAmazonS3 AmazonS3 { get; private set; }
        public SnsEventVerifier<ApplicationSns> SnsVerifer { get; private set; }

        public DynamoDbMockWebApplicationFactory(List<TableDef> tables)
        {
            _tables = tables;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
                .UseStartup<Startup>();

            builder.ConfigureServices(services =>
            {
                services.ConfigureDynamoDB();
                services.ConfigureSns();
                services.ConfigureS3();

                var serviceProvider = services.BuildServiceProvider();
                DynamoDb = serviceProvider.GetRequiredService<IAmazonDynamoDB>();
                DynamoDbContext = serviceProvider.GetRequiredService<IDynamoDBContext>();
                SimpleNotificationService = serviceProvider.GetRequiredService<IAmazonSimpleNotificationService>();

                var localstackS3Url = Environment.GetEnvironmentVariable("Localstack_SnsServiceUrl");
                AmazonS3 = new AmazonS3Client(new AmazonS3Config()
                {
                    ServiceURL = localstackS3Url,
                    AuthenticationRegion = "eu-west-2"
                });

                var localstackSnsUrl = Environment.GetEnvironmentVariable("Localstack_SnsServiceUrl");
                AmazonSQS = new AmazonSQSClient(new AmazonSQSConfig() { ServiceURL = localstackSnsUrl, AuthenticationRegion = "eu-west-2" });

                CreateSnsTopic();
                CreateNovaletSnsTopic();
                CreateS3Bucket();
                CreateDynamoDbTable();
            });

            // lets not send emails, but mock them
            builder.ConfigureTestServices(services =>
            {
                services.AddTransient(x => new Mock<INotificationClient>().Object);
            });
        }

        private void CreateDynamoDbTable()
        {
            var housingRegIndex = new GlobalSecondaryIndex
            {
                IndexName = "HousingRegIndex",
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = (long) 10,
                    WriteCapacityUnits = (long) 1
                },
                Projection = new Projection { ProjectionType = "ALL" }
            };

            foreach (var table in _tables)
            {
                try
                {
                    /*var indexKeySchema = new List<KeySchemaElement> {
                        {new KeySchemaElement { AttributeName = "status", KeyType = "HASH"}},  //Partition key
                        {new KeySchemaElement{ AttributeName = "submittedAt", KeyType = "RANGE"}}  //Sort key
                    };*/

                    //housingRegIndex.KeySchema = indexKeySchema;

                    var request = new CreateTableRequest(table.Name,
                        new List<KeySchemaElement> { new KeySchemaElement(table.KeyName, KeyType.HASH) },
                        new List<AttributeDefinition> { new AttributeDefinition(table.KeyName, table.KeyType) },
                        new ProvisionedThroughput(3, 3)
                        );
                    //request.GlobalSecondaryIndexes.Add(housingRegIndex);
                    _ = DynamoDb.CreateTableAsync(request).GetAwaiter().GetResult();
                }
                catch (ResourceInUseException)
                {
                    // It already exists :-)
                }
            }
        }

        private void CreateSnsTopic()
        {
            var snsAttrs = new Dictionary<string, string>
            {
                { "fifo_topic", "true" },
                { "content_based_deduplication", "true" }
            };

            var response = SimpleNotificationService.CreateTopicAsync(new CreateTopicRequest
            {
                Name = "housingRegisterTopic",
                Attributes = snsAttrs,
            }).Result;

            Environment.SetEnvironmentVariable("HOUSINGREGISTER_SNS_ARN", response.TopicArn);
            SnsVerifer = new SnsEventVerifier<ApplicationSns>(AmazonSQS, SimpleNotificationService, response.TopicArn);
        }

        private void CreateNovaletSnsTopic()
        {
            var snsAttrs = new Dictionary<string, string>
            {
                { "fifo_topic", "false" }
            };

            var response = SimpleNotificationService.CreateTopicAsync(new CreateTopicRequest
            {
                Name = "novaletexport-sns-topic",
                Attributes = snsAttrs,
            }).Result;

            Environment.SetEnvironmentVariable("NOVALET_SNS_ARN", response.TopicArn);
            SnsVerifer = new SnsEventVerifier<ApplicationSns>(AmazonSQS, SimpleNotificationService, response.TopicArn);
        }

        private void CreateS3Bucket()
        {
            string bucketName = "housing-export-bucket";

            AmazonS3.EnsureBucketExistsAsync(bucketName).ConfigureAwait(false).GetAwaiter().GetResult();

            Environment.SetEnvironmentVariable("HOUSINGREGISTER_EXPORT_BUCKET_NAME", bucketName);
        }
    }
}
