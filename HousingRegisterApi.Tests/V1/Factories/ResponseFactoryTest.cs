using FluentAssertions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        //TODO: add assertions for all the fields being mapped in `ResponseFactory.ToResponse()`. Also be sure to add test cases for
        // any edge cases that might exist.
        [Test]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var domain = new Entity();
            var response = domain.ToResponse();

            //TODO: check here that all of the fields have been mapped correctly. i.e. response.fieldOne.Should().Be("one")
            response.Id.Should().Be(domain.Id);
            response.CreatedAt.Should().Be(domain.CreatedAt.FormatDate());
        }
    }
}
