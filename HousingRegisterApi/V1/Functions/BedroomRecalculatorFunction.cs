using Amazon.Lambda.Core;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Notify.Client;
using Notify.Interfaces;
using System;

namespace HousingRegisterApi.V1.Functions
{
    public class BedroomRecalculatorFunction : BaseFunction, ILambdaFunctionHandler
    {
        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public BedroomRecalculatorFunction()
        {

        }

        public void Handle(ILambdaContext context)
        {
            context.Logger.LogLine("Recalculating bedroom need");
            var useCase = ServiceProvider.GetService<IRecalculateBedroomsUseCase>();
            bool result = useCase.Execute();
            string logResult = result == true ? "successful" : "unsuccessful";
            context.Logger.LogLine($"Recalculating was {logResult}");
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureDynamoDB();

            services.AddHttpClient();

            services.AddScoped<IApplicationApiGateway, DynamoDbGateway>();
            services.AddScoped<IRecalculateBedroomsUseCase, RecalculateBedroomsUseCase>();
            services.AddScoped<ISHA256Helper, SHA256Helper>();
            services.AddScoped<IVerifyCodeGenerator, VerifyCodeGenerator>();
            services.AddScoped<IBedroomCalculatorService, BedroomCalculatorService>();
            services.AddScoped<INotifyGateway, NotifyGateway>();
            services.AddTransient<INotificationClient>(x => new NotificationClient(Environment.GetEnvironmentVariable("NOTIFY_API_KEY")));
            services.AddScoped<ISnsGateway, ApplicationSnsGateway>();
            services.AddScoped<ISnsFactory, ApplicationSnsFactory>();

            base.ConfigureServices(services);
        }
    }
}
