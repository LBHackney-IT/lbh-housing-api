using System;
using HousingRegisterApi.V1.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace HousingRegisterApi.Tests.V1.Domain
{
    [TestFixture]
    public class EntityTests
    {
        [Test]
        public void EntitiesHaveAnId()
        {
            var entity = new Application();
            var guid = Guid.NewGuid();
            entity.Id = guid;
            entity.Id.Should().Be(guid);
        }

        [Test]
        public void EntitiesHaveACreatedAt()
        {
            var entity = new Application();
            var date = new DateTime(2019, 02, 21);
            entity.CreatedAt = date;

            entity.CreatedAt.Should().BeSameDateAs(date);
        }

        [Test]
        public void EntitesHaveANullSubmittedAt()
        {
            var entity = new Application();

            entity.SubmittedAt.Should().Be(null);
        }

        [Test]
        public void EntitiesHaveASubmittedAt()
        {
            var entity = new Application();
            var date = new DateTime(2019, 02, 21);
            entity.SubmittedAt = date;

            entity.SubmittedAt.Should().BeSameDateAs(date);
        }
    }
}
