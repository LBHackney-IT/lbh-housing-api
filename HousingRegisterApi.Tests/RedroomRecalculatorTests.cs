using AutoFixture;
using FluentAssertions;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase;
using Moq;
using NUnit.Framework;
using System;

namespace HousingRegisterApi.Tests
{
    public class RedroomRecalculatorTests : DynamoDbIntegrationTests<Startup>
    {
        [Test]
        public void CreateVerifyCodeCallsGatewayWhenAnExistingApplicationIsIncomplete()
        {
            // Arrange
            var application = new BedroomRecalculator();

            application.Recalculate();
        }
    }
}
