using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HousingRegisterApi.V1.Infrastructure
{
    // TODO: Use Hackney.Core NuGet package...

    public static class DynamoDbInitilisationExtensions
    {
        public static void ConfigureDynamoDB(this IServiceCollection services, TimeSpan? requestTimeout = null)
        {
            _ = bool.TryParse(Environment.GetEnvironmentVariable("DynamoDb_LocalMode"), out var localMode);

            if (localMode)
            {
                var url = Environment.GetEnvironmentVariable("DynamoDb_LocalServiceUrl");
                services.AddSingleton<IAmazonDynamoDB>(sp =>
                {
                    var clientConfig = new AmazonDynamoDBConfig { ServiceURL = url };
                    ApplyRequestTimeout(clientConfig, requestTimeout);
                    return new AmazonDynamoDBClient(clientConfig);
                });
            }
            else if (requestTimeout.HasValue)
            {
                // The SDK default is 100s. API Gateway stops at 29s, so a stuck
                // call has to fail while the route can still return an error.
                services.AddSingleton<IAmazonDynamoDB>(_ =>
                {
                    var clientConfig = new AmazonDynamoDBConfig
                    {
                        Timeout = requestTimeout,
                        MaxErrorRetry = 2,
                    };
                    return new AmazonDynamoDBClient(clientConfig);
                });
            }
            else
            {
                services.AddAWSService<IAmazonDynamoDB>();
            }

            services.AddScoped<IDynamoDBContext>(sp =>
            {
                var db = sp.GetService<IAmazonDynamoDB>();
                return new DynamoDBContext(db);
            });
        }

        private static void ApplyRequestTimeout(AmazonDynamoDBConfig clientConfig, TimeSpan? requestTimeout)
        {
            if (!requestTimeout.HasValue)
            {
                return;
            }

            clientConfig.Timeout = requestTimeout;
            clientConfig.MaxErrorRetry = 2;
        }
    }
}
