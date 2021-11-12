using AutoFixture;
using HousingRegisterApi.V1.Boundary.Request;
using System;

namespace HousingRegisterApi.Tests.V1.E2ETests.Fixtures
{
    public class AuthFixture
    {
        private readonly Fixture _fixture = new Fixture();

        /// <summary>
        /// Method to construct <see cref="CreateAuthRequest"/> that can be used in a test
        /// </summary>        
        /// <returns></returns>
        public CreateAuthRequest ConstructCreateAuthRequestRequest()
        {
            var entity = _fixture.Create<CreateAuthRequest>();
            return entity;
        }

        /// <summary>
        /// Method to construct <see cref="VerifyAuthRequest"/> that can be used in a test
        /// </summary>        
        /// <returns></returns>
        public VerifyAuthRequest ConstructVerifyAuthRequestRequest()
        {
            var entity = _fixture.Create<VerifyAuthRequest>();
            return entity;
        }
    }
}
