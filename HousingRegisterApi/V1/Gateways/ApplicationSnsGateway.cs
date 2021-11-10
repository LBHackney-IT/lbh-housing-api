using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using HousingRegisterApi.V1.Domain.Sns;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace HousingRegisterApi.V1.Gateways
{
    public class ApplicationSnsGateway : ISnsGateway
    {
        private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApplicationSnsGateway(IAmazonSimpleNotificationService amazonSimpleNotificationService)
        {
            _amazonSimpleNotificationService = amazonSimpleNotificationService;
            _jsonOptions = CreateJsonOptions();
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }

        public async Task Publish(ApplicationSns applicationSns)
        {
            string message = JsonSerializer.Serialize(applicationSns, _jsonOptions);
            var request = new PublishRequest
            {
                Message = message,
                TopicArn = Environment.GetEnvironmentVariable("HOUSINGREGISTER_SNS_ARN"),
                MessageGroupId = "SomeGroupId"
            };

            await _amazonSimpleNotificationService.PublishAsync(request).ConfigureAwait(false);
        }
    }
}
