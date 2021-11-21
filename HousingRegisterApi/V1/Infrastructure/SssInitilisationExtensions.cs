using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace HousingRegisterApi.V1.Infrastructure
{
    /// <summary>
    /// Initialise the Simple Storage Service 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SssInitilisationExtensions
    {
        public static void ConfigureS3(this IServiceCollection services)
        {
            _ = bool.TryParse(Environment.GetEnvironmentVariable("DynamoDb_LocalMode"), out var localMode);

            if (localMode)
            {
                var snsUrl = Environment.GetEnvironmentVariable("Localstack_SnsServiceUrl");

                services.TryAddSingleton<IAmazonS3>(sp =>
                {
                    var clientConfig = new AmazonS3Config
                    {
                        ServiceURL = snsUrl,
                        AuthenticationRegion = "eu-west-2",
                        UseHttp = true,
                        ForcePathStyle = true,
                    };

                    return new AmazonS3Client(clientConfig);
                });
            }
            else
            {
                services.TryAddScoped<IAmazonS3, AmazonS3Client>();
            }
        }
    }
}
