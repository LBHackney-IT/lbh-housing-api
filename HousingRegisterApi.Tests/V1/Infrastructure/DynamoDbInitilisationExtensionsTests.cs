using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using HousingRegisterApi.V1.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class DynamoDbInitilisationExtensionsTests
    {
        [TestCase(null)]
        [TestCase("false")]
        [TestCase("true")]
        public void ConfigureDynamoDBTestNoLocalModeEnvVarUsesAWSService(string localModeEnvVar)
        {
            Environment.SetEnvironmentVariable("DynamoDb_LocalMode", localModeEnvVar);

            ServiceCollection services = new ServiceCollection();
            services.ConfigureDynamoDB();

            services.Any(x => x.ServiceType == typeof(IAmazonDynamoDB)).Should().BeTrue();
            services.Any(x => x.ServiceType == typeof(IDynamoDBContext)).Should().BeTrue();

            Environment.SetEnvironmentVariable("DynamoDb_LocalMode", null);
        }

        [Test]
        public void ConfigureDynamoDB_WithTimeout_NonLocalMode_SetsTimeoutAndMaxErrorRetryOnClient()
        {
            // Resolving IAmazonDynamoDB just constructs the client object — no network call is made.
            Environment.SetEnvironmentVariable("DynamoDb_LocalMode", "false");
            var timeout = TimeSpan.FromSeconds(15);

            var services = new ServiceCollection();
            services.ConfigureDynamoDB(timeout);
            var client = services.BuildServiceProvider().GetRequiredService<IAmazonDynamoDB>();

            client.Config.Timeout.Should().Be(timeout);
            client.Config.MaxErrorRetry.Should().Be(2);

            Environment.SetEnvironmentVariable("DynamoDb_LocalMode", null);
        }

        [Test]
        public void ConfigureDynamoDB_WithTimeout_LocalMode_SetsTimeoutAndMaxErrorRetryOnClient()
        {
            Environment.SetEnvironmentVariable("DynamoDb_LocalMode", "true");
            Environment.SetEnvironmentVariable("DynamoDb_LocalServiceUrl", "http://localhost:8000");
            var timeout = TimeSpan.FromSeconds(15);

            var services = new ServiceCollection();
            services.ConfigureDynamoDB(timeout);
            var client = services.BuildServiceProvider().GetRequiredService<IAmazonDynamoDB>();

            client.Config.Timeout.Should().Be(timeout);
            client.Config.MaxErrorRetry.Should().Be(2);

            Environment.SetEnvironmentVariable("DynamoDb_LocalMode", null);
            Environment.SetEnvironmentVariable("DynamoDb_LocalServiceUrl", null);
        }

        [Test]
        public void ConfigureDynamoDB_WithoutTimeout_NonLocalMode_RegistersDefaultAWSClient()
        {
            // Without a timeout the method delegates to AddAWSService<IAmazonDynamoDB>, which
            // uses SDK defaults (MaxErrorRetry = 10, Timeout = 100 s). We only assert the
            // descriptor is registered here because resolving the default AWS client requires
            // credentials that are absent in the test environment.
            Environment.SetEnvironmentVariable("DynamoDb_LocalMode", "false");

            var services = new ServiceCollection();
            services.ConfigureDynamoDB();

            services.Any(x => x.ServiceType == typeof(IAmazonDynamoDB)).Should().BeTrue();
            services.Any(x => x.ServiceType == typeof(IDynamoDBContext)).Should().BeTrue();

            Environment.SetEnvironmentVariable("DynamoDb_LocalMode", null);
        }
    }
}
