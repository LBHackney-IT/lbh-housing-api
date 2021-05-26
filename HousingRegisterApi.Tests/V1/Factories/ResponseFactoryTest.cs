using FluentAssertions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        [Test]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var domain = new Application();
            var response = domain.ToResponse();

            response.Id.Should().Be(domain.Id);
            response.Status.Should().Be(domain.Status);
            response.CreatedAt.Should().Be(domain.CreatedAt);
            response.MainApplicant.Should().BeEquivalentTo(domain.MainApplicant);
            response.OtherMembers.Should().BeEquivalentTo(domain.OtherMembers);
        }
    }
}
