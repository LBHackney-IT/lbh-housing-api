using Amazon;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using dotenv.net;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Hackney.Core.Logging;
using HousingRegisterApi.V1;
using HousingRegisterApi.V1.Controllers;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase;
using HousingRegisterApi.V1.UseCase.Interfaces;
using HousingRegisterApi.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Notify.Client;
using Notify.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HousingRegisterApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AWSSDKHandler.RegisterXRayForAllServices();
        }

        public IConfiguration Configuration { get; }
        private static List<ApiVersionDescription> _apiVersions { get; set; }
        private const string ApiName = "housing-register";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true; // assume that the caller wants the default version if they don't specify
                o.ApiVersionReader = new UrlSegmentApiVersionReader(); // read the version number from the url segment header)
            });

            services.AddSingleton<IApiVersionDescriptionProvider, DefaultApiVersionDescriptionProvider>();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Token",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Your Hackney API Key",
                        Name = "X-Api-Key",
                        Type = SecuritySchemeType.ApiKey
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Token" }
                        },
                        new List<string>()
                    }
                });

                //Looks at the APIVersionAttribute [ApiVersion("x")] on controllers and decides whether or not
                //to include it in that version of the swagger document
                //Controllers must have this [ApiVersion("x")] to be included in swagger documentation!!
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    apiDesc.TryGetMethodInfo(out var methodInfo);

                    var versions = methodInfo?
                        .DeclaringType?.GetCustomAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions).ToList();

                    return versions?.Any(v => $"{v.GetFormattedApiVersion()}" == docName) ?? false;
                });

                //Get every ApiVersion attribute specified and create swagger docs for them
                foreach (var apiVersion in _apiVersions)
                {
                    var version = $"v{apiVersion.ApiVersion}";
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        Title = $"{ApiName}-api {version}",
                        Version = version,
                        Description = $"{ApiName} version {version}. Please check older versions for depreciated endpoints."
                    });
                }

                c.CustomSchemaIds(x => x.FullName);
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            services.ConfigureLambdaLogging(Configuration);
            AWSXRayRecorder.InitializeInstance(Configuration);
            AWSXRayRecorder.RegisterLogger(LoggingOptions.SystemDiagnostics);

            DotEnv.Fluent()
                .WithTrimValues()
                .WithProbeForEnv(probeLevelsToSearch: 5)
                .Load();

            var options = ApiOptions.FromEnv();
            services.AddSingleton(x => options);

            services.ConfigureDynamoDB();
            services.ConfigureSns();
            services.ConfigureS3();
            services.AddTokenFactory();
            services.AddHttpContextWrapper();
            services.AddHttpContextAccessor();

            RegisterGateways(services);
            RegisterUseCases(services);
            RegisterServices(services);
        }

        private static void RegisterGateways(IServiceCollection services)
        {
            services.AddScoped<IApplicationApiGateway, DynamoDbGateway>();
            services.AddScoped<INotifyGateway, NotifyGateway>();
            services.AddScoped<IActivityGateway, ApplicationActivityGateway>();
            services.AddScoped<ISnsGateway, ApplicationSnsGateway>();
            services.AddScoped<ISnsFactory, ApplicationSnsFactory>();
            services.AddScoped<IFileGateway, FileExportGateway>();

            services.AddTransient<INotificationClient>(x => new NotificationClient(Environment.GetEnvironmentVariable("NOTIFY_API_KEY")));
            services.AddHttpClient<IEvidenceApiGateway, EvidenceApiGateway>();
        }

        private static void RegisterUseCases(IServiceCollection services)
        {
            services.AddScoped<ICompleteApplicationUseCase, CompleteApplicationUseCase>();
            services.AddScoped<ICreateNewApplicationUseCase, CreateNewApplicationUseCase>();
            services.AddScoped<IGetAllApplicationsUseCase, GetAllApplicationsUseCase>();
            services.AddScoped<IGetApplicationByIdUseCase, GetApplicationByIdUseCase>();
            services.AddScoped<IUpdateApplicationUseCase, UpdateApplicationUseCase>();
            services.AddScoped<IRecalculateBedroomsUseCase, RecalculateBedroomsUseCase>();
            services.AddScoped<ICreateEvidenceRequestUseCase, CreateEvidenceRequestUseCase>();
            services.AddScoped<ICreateAuthUseCase, CreateAuthUseCase>();
            services.AddScoped<IVerifyAuthUseCase, VerifyAuthUseCase>();
            services.AddScoped<ICalculateBedroomsUseCase, CalculateBedroomsUseCase>();
            services.AddScoped<ICreateNovaletExportUseCase, CreateNovaletExportUseCase>();
            services.AddScoped<IGetNovaletExportUseCase, GetNovaletExportUseCase>();
            services.AddScoped<IListNovaletExportFilesUseCase, ListNovaletExportFilesUseCase>();
            services.AddScoped<IApproveNovaletExportUseCase, ApproveNovaletExportUseCase>();
            services.AddScoped<IAddApplicationNoteUseCase, AddApplicationNoteUseCase>();
            services.AddScoped<IViewingApplicationUseCase, ViewingApplicationUseCase>();
            services.AddScoped<IGetInternalReportUseCase, GetInternalReportUseCase>();

            services.AddScoped<ISHA256Helper, SHA256Helper>();
            services.AddScoped<IPaginationHelper, PaginationHelper>();
            services.AddScoped<IVerifyCodeGenerator, VerifyCodeGenerator>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IBiddingNumberGenerator, BiddingNumberGenerator>();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IBedroomCalculatorService, BedroomCalculatorService>();
            services.AddScoped<ICsvService, CsvGeneratorService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCorrelation();
            app.UseXRay("housing-register-api");

            //Get All ApiVersions,
            var api = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            _apiVersions = api.ApiVersionDescriptions.ToList();

            //Swagger ui to view the swagger.json file
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersionDescription in _apiVersions)
                {
                    //Create a swagger endpoint for each swagger version
                    c.SwaggerEndpoint($"{apiVersionDescription.GetFormattedApiVersion()}/swagger.json",
                        $"{ApiName}-api {apiVersionDescription.GetFormattedApiVersion()}");
                }
            });
            app.UseSwagger();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                // SwaggerGen won't find controllers that are routed via this technique.
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
