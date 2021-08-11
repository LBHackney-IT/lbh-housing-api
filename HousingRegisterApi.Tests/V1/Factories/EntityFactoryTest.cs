using AutoFixture;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Factories
{
    [TestFixture]
    public class EntityFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var databaseEntity = _fixture.Create<ApplicationDbEntity>();
            var entity = databaseEntity.ToDomain();

            databaseEntity.Id.Should().Be(entity.Id);
            databaseEntity.Status.Should().Be(entity.Status);
            databaseEntity.Reference.Should().Be(entity.Reference);
            databaseEntity.CreatedAt.Should().BeSameDateAs(entity.CreatedAt);
            databaseEntity.MainApplicant.Should().BeEquivalentTo(entity.MainApplicant);
            databaseEntity.OtherMembers.Should().BeEquivalentTo(entity.OtherMembers);
        }

        [Test]
        public void CanMapADomainEntityToADatabaseObject()
        {
            var entity = _fixture.Create<Application>();
            var databaseEntity = entity.ToDatabase();

            entity.Id.Should().Be(databaseEntity.Id);
            entity.Reference.Should().Be(databaseEntity.Reference);
            entity.Status.Should().Be(databaseEntity.Status);
            entity.CreatedAt.Should().BeSameDateAs(databaseEntity.CreatedAt);
            entity.MainApplicant.Should().BeEquivalentTo(databaseEntity.MainApplicant);
            entity.OtherMembers.Should().BeEquivalentTo(databaseEntity.OtherMembers);
        }
    }
}
