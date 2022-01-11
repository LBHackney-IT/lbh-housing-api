using System;
using AutoFixture;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Infrastructure;

namespace HousingRegisterApi.Tests.V1.E2ETests.Fixtures
{
    public class ApplicationFixture
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly ISHA256Helper _hashHelper = new SHA256Helper();

        /// <summary>
        /// Method to construct <see cref="Application"/> that can be used in a test
        /// </summary>
        /// <returns></returns>
        public Application ConstructTestEntity()
        {
            var entity = _fixture.Create<Application>();
            entity.CreatedAt = DateTime.UtcNow;
            entity.SubmittedAt = null;
            entity.Reference = _hashHelper.Generate(entity.MainApplicant.ContactInformation.EmailAddress).Substring(0, 10);
            entity.ImportedFromLegacyDatabase = false;
            return entity;
        }

        /// <summary>
        /// Method to construct <see cref="CreateApplicationRequest"/> that can be used in a test
        /// </summary>
        /// <returns></returns>
        public CreateApplicationRequest ConstructCreateApplicationRequest()
        {
            var entity = _fixture.Create<CreateApplicationRequest>();
            return entity;
        }

        /// <summary>
        /// Method to construct <see cref="UpdateApplicationRequest"/> that can be used in a test
        /// </summary>
        /// <returns></returns>
        public UpdateApplicationRequest ConstructUpdateApplicationRequest()
        {
            var entity = _fixture.Create<UpdateApplicationRequest>();
            return entity;
        }

        // TODO: add some more variations and example test data
    }
}
