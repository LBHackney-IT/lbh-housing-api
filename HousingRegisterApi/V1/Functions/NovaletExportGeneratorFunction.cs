using Amazon.Lambda.Core;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HousingRegisterApi.V1.Functions
{
    public class NovaletExportGeneratorFunction : BaseFunction, ILambdaFunctionHandler
    {
        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public NovaletExportGeneratorFunction()
        {

        }

        public void Handle(ILambdaContext context)
        {
            context.Logger.LogLine("Generating Novalet export for approval");
            var createExportFileUseCase = ServiceProvider.GetService<ICreateNovaletExportUseCase>();

            var file = createExportFileUseCase.Execute().ConfigureAwait(false).GetAwaiter().GetResult();

            if (file != null)
            {
                context.Logger.LogLine($"Success");
            }
            else
            {
                context.Logger.LogLine($"File was not generated");
            }
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureDynamoDB();
            services.ConfigureS3();

            services.AddHttpClient();

            services.AddScoped<IApplicationApiGateway, DynamoDbGateway>();
            services.AddScoped<ISHA256Helper, SHA256Helper>();
            services.AddScoped<IVerifyCodeGenerator, VerifyCodeGenerator>();
            services.AddScoped<IBedroomCalculatorService, BedroomCalculatorService>();
            services.AddScoped<ICsvService, CsvGeneratorService>();
            services.AddScoped<IFileGateway, FileExportGateway>();
            services.AddScoped<ICreateNovaletExportUseCase, CreateNovaletExportUseCase>();

            base.ConfigureServices(services);
        }
    }
}
