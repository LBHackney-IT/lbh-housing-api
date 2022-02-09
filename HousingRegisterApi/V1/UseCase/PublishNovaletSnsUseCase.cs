using Amazon.SimpleNotificationService.Model;
using HousingRegisterApi.V1.Domain.Sns;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase
{
    public class PublishNovaletSnsUseCase : IPublishNovaletSnsUseCase
    {
        private readonly ILogger<PublishNovaletSnsUseCase> _logger;
        private readonly INovaletSnsGateway _novaletSnsGateway;

        public PublishNovaletSnsUseCase(
            ILogger<PublishNovaletSnsUseCase> logger,
            INovaletSnsGateway novaletSnsGateway)
        {
            _logger = logger;
            _novaletSnsGateway = novaletSnsGateway;
        }

        public Task<PublishResponse> Execute()
        {
            string fileName = $"LBH-APPLICANT FEED-{DateTime.UtcNow:ddMMyyyy}.csv";

            _logger.LogInformation($"Pushing message to SNS for file {fileName}");
            NovaletSns novaletSns = new NovaletSns();
            novaletSns.FileName = fileName;
            novaletSns.InvokeDateTime = DateTime.Now;

            return _novaletSnsGateway.Publish(novaletSns);
        }
    }
}
