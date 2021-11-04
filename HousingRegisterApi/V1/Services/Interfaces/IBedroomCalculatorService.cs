using HousingRegisterApi.V1.Domain;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Services
{
    public interface IBedroomCalculatorService
    {
        int Calculate(IEnumerable<Applicant> household);
    }
}
