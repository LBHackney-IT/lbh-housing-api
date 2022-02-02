using Amazon.Lambda.Core;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HousingRegisterApi.V1.Functions
{
    public class FtpUploadNovaletExportFunction : BaseFunction, ILambdaFunctionHandler
    {
        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public FtpUploadNovaletExportFunction()
        {

        }

        public void Handle(ILambdaContext context)
        {
            context.Logger.LogLine("Generating Novalet export for approval");
            var ftpNovaletUploadUseCase = ServiceProvider.GetService<IFtpNovaletUploadUseCase>();

            var uploaded = ftpNovaletUploadUseCase.Execute().ConfigureAwait(false).GetAwaiter().GetResult();

            if (uploaded)
            {
                context.Logger.LogLine($"FTP upload successful");
            }
            else
            {
                context.Logger.LogLine($"Novalet file was not uploaded to ftp");
            }
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureS3();
            services.AddHttpClient();

            services.AddScoped<IFileGateway, FileExportGateway>();
            services.AddScoped<IFtpHelper, FtpHelper>();

            base.ConfigureServices(services);
        }
    }
}
