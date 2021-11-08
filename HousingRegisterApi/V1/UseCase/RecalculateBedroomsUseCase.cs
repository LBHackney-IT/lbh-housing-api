using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace HousingRegisterApi.V1.UseCase
{
    public class RecalculateBedroomsUseCase : IRecalculateBedroomsUseCase
    {
        private readonly ILogger _logger;
        private readonly IApplicationApiGateway _gateway;
        private readonly IBedroomCalculatorService _bedroomCalculatorService;
        private readonly INotifyGateway _notifyGateway;

        public RecalculateBedroomsUseCase(
            ILogger<RecalculateBedroomsUseCase> logger,
            IApplicationApiGateway gateway,
            IBedroomCalculatorService bedroomCalculatorService,
            INotifyGateway notifyGateway)
        {
            _logger = logger;
            _gateway = gateway;
            _bedroomCalculatorService = bedroomCalculatorService;
            _notifyGateway = notifyGateway;
        }

        public void Execute()
        {
            var applications = _gateway.GetApplicationsAtStatus("Submitted", "Active", "ActiveUnderAppeal");

            // code comes here if applications are found
            _logger.LogInformation($"Recalculating bedrooms needed for {applications.Count()} applications");

            foreach (var application in applications)
            {
                try
                {
                    // have to use calculated bedroom need is null for backward compatibility,
                    // but, at this point, it should have a value
                    int? currentBedroomNeed = application.CalculatedBedroomNeed;
                    int? newBedroomNeed = _bedroomCalculatorService.Calculate(application);

                    if (newBedroomNeed == null)
                    {
                        _logger.LogInformation($"Unable to recalculate bedroom need for application: {application.Id}");
                    }
                    else if (newBedroomNeed == currentBedroomNeed)
                    {
                        _logger.LogInformation($"No bedroom changes for application: {application.Id}");
                    }
                    else 
                    {
                        _gateway.UpdateApplication(application.Id, new UpdateApplicationRequest());
                        _logger.LogInformation($"Bedroom need for application {application.Id} recalculated from '{currentBedroomNeed}' to '{newBedroomNeed}'");
                        _notifyGateway.NotifyResidentOfBedroomChange(application.MainApplicant.ContactInformation.EmailAddress, currentBedroomNeed, newBedroomNeed.Value);
                    }

                }
                catch (Exception exp)
                {
                    _logger.LogError(exp, $"Error processing application {application.Id}");
                }

            }
        }
    }
}
