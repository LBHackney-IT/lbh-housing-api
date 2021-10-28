using BedroomCalculator.Configuration;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace BedroomCalculator
{
    class Program
    {
        public static void Main()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environmentName}.json", false, true)
                .AddEnvironmentVariables()
                .Build();
                     
            IServiceCollection serviceCollection = new ServiceCollection();
            IServiceProvider serviceProvider = serviceCollection.AddConfiguration(configuration);

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Processing applications");

            var recalculateBedroomsUseCase = serviceProvider.GetRequiredService<IRecalculateBedroomsUseCase>();
            recalculateBedroomsUseCase.Execute();

            logger.LogInformation("Processing finished");
        }
    }
}
