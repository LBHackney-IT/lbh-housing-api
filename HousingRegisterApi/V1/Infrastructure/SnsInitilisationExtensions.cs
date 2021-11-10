using Amazon.SimpleNotificationService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace HousingRegisterApi.V1.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class SnsInitilisationExtensions
    {
        public static void ConfigureSns(this IServiceCollection services)
        {
            bool localMode = false;
            _ = bool.TryParse(Environment.GetEnvironmentVariable("DynamoDb_LocalMode"), out localMode);

            if (localMode)
            {              
                var snsUrl = Environment.GetEnvironmentVariable("Localstack_SnsServiceUrl");
            
                services.TryAddSingleton<IAmazonSimpleNotificationService>(sp =>
                {
                    var clientConfig = new AmazonSimpleNotificationServiceConfig { ServiceURL = snsUrl };
                    return new AmazonSimpleNotificationServiceClient(clientConfig);
                });
            }
            else
            {
                services.TryAddScoped<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>();               
            }           
        }
    }
}
