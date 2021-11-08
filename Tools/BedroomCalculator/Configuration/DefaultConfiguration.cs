using Amazon;
using Amazon.XRay.Recorder.Core;
using dotenv.net;
using HousingRegisterApi.V1;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notify.Client;
using Notify.Interfaces;
using System;

namespace BedroomCalculator.Configuration
{
    public static class DefaultConfiguration
    {
        public static IServiceProvider AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);

            ConfigureLogging(services, configuration);
            AWSXRayRecorder.InitializeInstance(configuration);
            AWSXRayRecorder.RegisterLogger(LoggingOptions.SystemDiagnostics);

            DotEnv.Fluent()
                .WithTrimValues()
                .WithProbeForEnv(probeLevelsToSearch: 5)
                .Load();

            var options = ApiOptions.FromEnv();
            services.AddSingleton(x => options);

            services.ConfigureDynamoDB();
            RegisterGateways(services);
            RegisterUseCases(services);
            RegisterServices(services);

            var provider = services.BuildServiceProvider();

            return provider;
        }

        private static void ConfigureLogging(IServiceCollection services, IConfiguration configuration)
        {
            // We rebuild the logging stack so as to ensure the console logger is not used in production.
            // See here: https://weblog.west-wind.com/posts/2018/Dec/31/Dont-let-ASPNET-Core-Default-Console-Logging-Slow-your-App-down
            services.AddLogging(config =>
            {
                // clear out default configuration
                config.ClearProviders();

                config.AddConfiguration(configuration.GetSection("Logging"));
                config.AddDebug();
                config.AddEventSourceLogger();

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
                {
                    config.AddConsole();
                }
            });
        }

        private static void RegisterGateways(IServiceCollection services)
        {
            services.AddScoped<IApplicationApiGateway, DynamoDbGateway>();
            services.AddScoped<INotifyGateway, NotifyGateway>();
            services.AddTransient<INotificationClient>(x => new NotificationClient(Environment.GetEnvironmentVariable("NOTIFY_API_KEY")));
        }

        private static void RegisterUseCases(IServiceCollection services)
        {
            services.AddScoped<IRecalculateBedroomsUseCase, RecalculateBedroomsUseCase>();

            services.AddScoped<ISHA256Helper, SHA256Helper>();
            services.AddScoped<IVerifyCodeGenerator, VerifyCodeGenerator>();
        }
        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IBedroomCalculatorService, BedroomCalculatorService>();
        }
    }
}
