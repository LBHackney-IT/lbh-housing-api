using System;
using HousingRegisterApi.Tests.V1.E2ETests.Fixtures;
using HousingRegisterApi.V1;

namespace HousingRegisterApi.Tests
{
    /// <summary>
    /// Wires the API host for DynamoDB integration tests.
    /// </summary>
    /// <remarks>
    /// You do not need to set <c>HACKNEY_JWT_SECRET</c> yourself to run these tests.
    /// <see cref="Apply"/> is called automatically by <see cref="DynamoDbIntegrationTests{TStartup}"/>
    /// and <see cref="DynamoDbMockWebApplicationFactory{TStartup}"/> before the host starts.
    ///
    /// The API reads <c>HACKNEY_JWT_SECRET</c> at runtime (e.g. resident token generation in
    /// <c>TokenGenerator</c>), so the test host must expose that variable name. The value applied
    /// here is always <see cref="E2eStaffTokenHelper.TestJwtSecret"/> — a fixed dummy string used
    /// only in tests. It is not the production secret from SSM and must never be treated as one.
    /// </remarks>
    internal static class E2eHostConfiguration
    {
        internal static void Apply()
        {
            Environment.SetEnvironmentVariable("AUTHORISED_OFFICER_GROUP", E2eStaffTokenHelper.StaffGroup);

            // Test-only value; overwrites any local .env so E2E never depends on a real JWT secret.
            Environment.SetEnvironmentVariable("HACKNEY_JWT_SECRET", E2eStaffTokenHelper.TestJwtSecret);
        }

        internal static ApiOptions CreateApiOptions()
        {
            var options = ApiOptions.FromEnv();
            options.HackneyJwtSecret = E2eStaffTokenHelper.TestJwtSecret;
            options.AuthorisedOfficerGroup = E2eStaffTokenHelper.StaffGroup;
            return options;
        }
    }
}
