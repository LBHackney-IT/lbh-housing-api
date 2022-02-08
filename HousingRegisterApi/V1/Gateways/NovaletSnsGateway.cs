using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using HousingRegisterApi.V1.Domain.Sns;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Gateways
{
    public class NovaletSnsGateway : INovaletSnsGateway
    {
        private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
        private readonly JsonSerializerOptions _jsonOptions;

        public NovaletSnsGateway(IAmazonSimpleNotificationService amazonSimpleNotificationService)
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

        public Task<PublishResponse> Publish(NovaletSns novaletSns)
        {
            string message = JsonSerializer.Serialize(novaletSns, _jsonOptions);
            var request = new PublishRequest
            {
                Message = message,
                TopicArn = Environment.GetEnvironmentVariable("NOVALET_SNS_ARN"),
                MessageGroupId = "SomeGroupId"
            };

            return _amazonSimpleNotificationService.PublishAsync(request);
        }
    }
}
