using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HousingRegisterApi
{
    public class BedroomRecalculator
    {
        private readonly ServiceProvider _serviceProvider;

        public BedroomRecalculator()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        public void Recalculate()
        {
            _serviceProvider.GetService<IRecalculateBedroomsUseCase>().Execute();
        }

        /// <summary>
        /// Configure whatever dependency injection you like here
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureServices(IServiceCollection services)
        {
            // add dependencies here ex: Logging, IMemoryCache, Interface mapping to concrete class, etc...

            // add a hook to your class that will actually do the application logic
            services.AddTransient<IRecalculateBedroomsUseCase>();
        }

        /// <summary>
        /// Since we don't want to dispose of the ServiceProvider in the FunctionHandler, we will
        /// at least try to clean up after ourselves in the destructor for the class.
        /// </summary>
        ~BedroomRecalculator()
        {
            _serviceProvider.Dispose();
        }
    }
}
