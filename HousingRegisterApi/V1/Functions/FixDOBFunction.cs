using Amazon.Lambda.Core;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Functions
{
    public class FixDOBFunction : BaseFunction
    {
        public FixDOBFunction() : base()
        {

        }
        protected async Task Handle(ILambdaContext context)
        {
            context.Logger.LogLine("Fixing DOBs");
            var useCase = ServiceProvider.GetService<IFixDOBUseCase>();
            await useCase.Execute().ConfigureAwait(false);
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureDynamoDB();

            services.AddScoped<IFixDOBUseCase, FixDOBUseCase>();

            base.ConfigureServices(services);
        }
    }
}
