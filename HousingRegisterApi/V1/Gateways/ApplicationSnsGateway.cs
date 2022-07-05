using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using HousingRegisterApi.V1.Domain.Sns;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
//using System.Text.Json;
//using System.Text.Json.Serialization;

namespace HousingRegisterApi.V1.Gateways
{
    public class ApplicationSnsGateway : ISnsGateway
    {
        private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
        //private readonly JsonSerializerOptions _jsonOptions;

        public ApplicationSnsGateway(IAmazonSimpleNotificationService amazonSimpleNotificationService)
        {
            _amazonSimpleNotificationService = amazonSimpleNotificationService;
            //_jsonOptions = CreateJsonOptions();
        }

        //private static JsonSerializerOptions CreateJsonOptions()
        //{
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //        WriteIndented = true
        //    };
        //    options.Converters.Add(new JsonStringEnumConverter());
        //    return options;
        //}

        DefaultContractResolver _contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        public void Publish(ApplicationSns applicationSns)
        {
            var options = new JsonSerializerSettings
            {
                ContractResolver = _contractResolver,
                Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = true } },
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string message = JsonConvert.SerializeObject(applicationSns, options);
            var request = new PublishRequest
            {
                Message = message,
                TopicArn = Environment.GetEnvironmentVariable("HOUSINGREGISTER_SNS_ARN"),
                MessageGroupId = "SomeGroupId"
            };

            _amazonSimpleNotificationService.PublishAsync(request);
        }
    }
}
