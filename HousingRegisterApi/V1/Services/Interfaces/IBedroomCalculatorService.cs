using HousingRegisterApi.V1.Domain;
using System;

namespace HousingRegisterApi.V1.Services
{
    public interface IBedroomCalculatorService
    {
        /// <summary>
        /// Calculates the required number of bedrooms
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        int Calculate(Application application);
    }
}
