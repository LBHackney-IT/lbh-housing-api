using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HousingRegisterApi.V1.UseCase
{
    public class CalculateBedroomsUseCase : ICalculateBedroomsUseCase
    {
        private readonly ILogger _logger;
        private readonly IApplicationApiGateway _gateway;
        private readonly IBedroomCalculatorService _bedroomCalculatorService;
        public CalculateBedroomsUseCase(
            ILogger<CalculateBedroomsUseCase> logger,
            IApplicationApiGateway gateway,
            IBedroomCalculatorService bedroomCalculatorService)
        {
            _logger = logger;
            _gateway = gateway;
            _bedroomCalculatorService = bedroomCalculatorService;
        }

        public SimpleTypeResponse<int> Execute(Guid id)
        {
            Application application = _gateway.GetApplicationById(id);
            int bedroom = _bedroomCalculatorService.Calculate(application);

            // lets log the scenario incase we need to test it if its wrong
            var household = new List<Applicant> { application.MainApplicant }.Concat(application.OtherMembers).ToList();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Calculated {bedroom} bedroom(s) for the following {household.Count} occupant(s)");
            household.ForEach(app =>
            {
                string applicantOrMember = string.IsNullOrWhiteSpace(app.Person.RelationshipType) ? "Applicant" : "Member";
                stringBuilder.AppendLine($"{applicantOrMember}:- Age:'{app.Person.Age}', Gender:'{app.Person.Gender}', Relation:'{app.Person.RelationshipType}'");
            });

            _logger.LogInformation(stringBuilder.ToString());

            return new SimpleTypeResponse<int>(bedroom);
        }
    }
}
