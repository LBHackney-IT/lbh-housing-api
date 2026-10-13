using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using HousingRegisterApi.V1.Domain.Sns;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Gateways
{
    public class ApplicationSnsGateway : ISnsGateway
    {
        private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
        private readonly ILogger<ApplicationSnsGateway> _logger;

        public ApplicationSnsGateway(
            IAmazonSimpleNotificationService amazonSimpleNotificationService,
            ILogger<ApplicationSnsGateway> logger)
        {
            _amazonSimpleNotificationService = amazonSimpleNotificationService;
            _logger = logger;
        }

        DefaultContractResolver _contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        public async Task Publish(ApplicationSns applicationSns)
        {
            var options = new JsonSerializerSettings
            {
                ContractResolver = _contractResolver,
                Converters = new List<JsonConverter> { new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() } },
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string message = JsonConvert.SerializeObject(applicationSns, options);
            var request = new PublishRequest
            {
                Message = message,
                TopicArn = Environment.GetEnvironmentVariable("HOUSING_REGISTER_SNS_ARN"),
                MessageGroupId = "SomeGroupId"
            };

            try
            {
                var sw = Stopwatch.StartNew();
                var response = await _amazonSimpleNotificationService.PublishAsync(request).ConfigureAwait(false);
                _logger.LogInformation("SNS ok {ApplicationId} {Id} {CorrelationId} {MessageId} {ElapsedMs}",
                    applicationSns.EntityId, applicationSns.Id, applicationSns.CorrelationId, response.MessageId, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SNS fail {ApplicationId} {Id} {CorrelationId}",
                    applicationSns.EntityId, applicationSns.Id, applicationSns.CorrelationId);
            }
        }
    }
}
