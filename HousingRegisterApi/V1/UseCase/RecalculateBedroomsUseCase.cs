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

        public RecalculateBedroomsUseCase(
            ILogger<RecalculateBedroomsUseCase> logger,
            IApplicationApiGateway gateway,
            IBedroomCalculatorService bedroomCalculatorService)
        {
            _logger = logger;
            _gateway = gateway;
            _bedroomCalculatorService = bedroomCalculatorService;
        }

        public void Execute()
        {
            var searchParameters = new SearchQueryParameter()
            {
                Status = "Submitted",
                HasAssessment = true,
            };

            _logger.LogInformation($"Searching for submitted applications with assessments");
            var applications = _gateway.GetApplications(searchParameters);

            if (!applications.Any())
            {
                _logger.LogInformation($"No applications found");
                return;
            }

            // code comes here if applications are found
            _logger.LogInformation($"Recalculating bedrooms needed for {applications.Count()} applications");

            foreach (var application in applications)
            {
                try
                {
                    if (application.Assessment == null)
                    {
                        throw new InvalidOperationException($"No assessment exists");
                    }

                    int? currentBedroomNeed = application.Assessment?.BedroomNeed ?? application.CalculatedBedroomNeed;
                    int? newBedroomNeed = _bedroomCalculatorService.Calculate(application);

                    // only update if required
                    if (newBedroomNeed != currentBedroomNeed)
                    {
                        application.Assessment.BedroomNeed = newBedroomNeed;

                        UpdateApplicationRequest updateRequest = new UpdateApplicationRequest()
                        {
                            Assessment = application.Assessment,
                            AssignedTo = application.AssignedTo,
                            MainApplicant = application.MainApplicant,
                            OtherMembers = application.OtherMembers,
                            SensitiveData = application.SensitiveData,
                            Status = application.Status,                           
                        };

                        _gateway.UpdateApplication(application.Id, updateRequest);
                        _logger.LogInformation($"Bedroom need changed for application {application.Id} from {currentBedroomNeed} to {newBedroomNeed}");
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
