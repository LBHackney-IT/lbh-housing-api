using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HousingRegisterApi
{
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        protected override IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostLambdaDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
