using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Hackney.Core.JWT;

namespace HousingRegisterApi.V1.UseCase
{
    public class RecalculateBedroomsUseCase : IRecalculateBedroomsUseCase
    {
        private readonly ILogger _logger;
        private readonly IApplicationApiGateway _gateway;
        private readonly IBedroomCalculatorService _bedroomCalculatorService;
        private readonly INotifyGateway _notifyGateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public RecalculateBedroomsUseCase(
            ILogger<RecalculateBedroomsUseCase> logger,
            IApplicationApiGateway gateway,
            IBedroomCalculatorService bedroomCalculatorService,
            INotifyGateway notifyGateway,
            ISnsGateway snsGateway,
            ISnsFactory snsFactory)
        {
            _logger = logger;
            _gateway = gateway;
            _bedroomCalculatorService = bedroomCalculatorService;
            _notifyGateway = notifyGateway;
            _snsFactory = snsFactory;
            _snsGateway = snsGateway;
        }

        public bool Execute()
        {
            bool success = true;

            var applications = _gateway.GetApplicationsAtStatus(0, 0, "Submitted", "Active", "ActiveUnderAppeal");

            // code comes here if applications are found
            _logger.LogInformation($"Recalculating bedrooms needed for {applications.Count()} applications");

            foreach (var application in applications)
            {
                try
                {
                    // have to use calculated bedroom need is null for backward compatibility,
                    // but, at this point, it should have a value
                    int? currentBedroomNeed = application.CalculatedBedroomNeed;
                    //Not sure if bands are to be recalcilated so setting these to the same for now
                    string currentBand = application.Assessment?.Band;

                    int? newBedroomNeed = _bedroomCalculatorService.Calculate(application);

                    if (newBedroomNeed == null)
                    {
                        _logger.LogInformation($"Unable to recalculate bedroom need for application: {application.Id}");
                    }
                    else if (currentBand == null)
                    {
                        _logger.LogInformation($"No band found for application: {application.Id}");
                    }
                    else if (newBedroomNeed == currentBedroomNeed)
                    {
                        _logger.LogInformation($"No bedroom changes for application: {application.Id}");
                    }
                    else if ((newBedroomNeed.HasValue && application.Assessment?.BedroomNeed.HasValue == true) && (newBedroomNeed != application.Assessment.BedroomNeed))
                    {
                        _logger.LogInformation($"Current bedroom need = {currentBedroomNeed} and newBedroomNeed = {newBedroomNeed}");
                        //Significant birthday found but bedroom need set manually so update application status
                        UpdateApplicationRequest updateRequest = new UpdateApplicationRequest();
                        updateRequest.Status = ApplicationStatus.AwaitingReassessment;
                        _gateway.UpdateApplication(application.Id, updateRequest);
                        _logger.LogInformation($"Application status for {application.Id} set to Awaiting Reassessment as bedroom need was manually set to {application.Assessment.BedroomNeed}");

                    }
                    else if (newBedroomNeed.HasValue && currentBedroomNeed.HasValue) //This assumes they should have had a previous bedroon need for it to be recalculated
                    {
                        //We are using most up-to-date template 2c854b56-6e3c-45e4-b005-961c065b989f and not old template in ticket HRT-337
                        //TODO: If we are doing band calculations. Would need to update band in the call below
                        _gateway.UpdateApplication(application.Id, new UpdateApplicationRequest());
                        _logger.LogInformation($"Bedroom need for application {application.Id} recalculated from '{currentBedroomNeed}' to '{newBedroomNeed}'");

                        _notifyGateway.NotifyResidentOfBedroomChange(application.MainApplicant.ContactInformation.EmailAddress, application.MainApplicant.Person.FullName, newBedroomNeed.Value, currentBand);

                        //Create a token for use in activity feed update
                        var token = new Token()
                        {
                            Name = application.MainApplicant.Person.FullName,
                            Email = application.MainApplicant.ContactInformation.EmailAddress,
                        };

                        var applicationSnsMessage = _snsFactory.Update(application.Id, currentBedroomNeed, newBedroomNeed, token);
                        _snsGateway.Publish(applicationSnsMessage);
                    }

                }
                catch (Exception exp)
                {
                    _logger.LogError(exp, $"Error processing application {application.Id}");

                    // carry on processing the rest
                    success = false;
                }
            }

            return success;
        }
    }
}
