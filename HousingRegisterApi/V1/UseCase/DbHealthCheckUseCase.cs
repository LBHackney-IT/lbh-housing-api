using System;
using System.Linq;
using HousingRegisterApi.V1.Boundary;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HousingRegisterApi.V1.UseCase
{
    public class DbHealthCheckUseCase
    {
        private readonly HealthCheckService _healthCheckService;

        public DbHealthCheckUseCase(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        public HealthCheckResponse Execute()
        {
            var result = _healthCheckService.CheckHealthAsync().Result;

            var success = result.Status == HealthStatus.Healthy;
            var message = string.Join(
                Environment.NewLine,
                result.Entries.Select(entry => $"{entry.Key}: {entry.Value.Description}"));
            return new HealthCheckResponse(success, message);
        }
    }

}
