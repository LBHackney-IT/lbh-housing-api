using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Services;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HousingRegisterApi.V1.UseCase
{
    public class CalculateBedroomsUseCase : ICalculateBedroomsUseCase
    {
        private readonly ILogger _logger;
        private readonly IBedroomCalculatorService _bedroomCalculatorService;
        public CalculateBedroomsUseCase(
            ILogger<CalculateBedroomsUseCase> logger,
            IBedroomCalculatorService bedroomCalculatorService)
        {
            _logger = logger;
            _bedroomCalculatorService = bedroomCalculatorService;
        }

        public SimpleTypeResponse<int> Execute(CalculateBedroomsRequest request)
        {
            var household = new List<Applicant> { request.MainApplicant }.Concat(request.OtherMembers).ToList();
            int bedroom = _bedroomCalculatorService.Calculate(household);

            // lets log the scenario incase we need to test it if its wrong
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
