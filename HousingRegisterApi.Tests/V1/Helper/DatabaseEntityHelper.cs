using AutoFixture;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Infrastructure;

namespace HousingRegisterApi.Tests.V1.Helper
{
    public static class DatabaseEntityHelper
    {
        public static ApplicationDbEntity CreateDatabaseEntity()
        {
            var entity = new Fixture().Create<Application>();
            return CreateDatabaseEntityFrom(entity);
        }

        public static ApplicationDbEntity CreateDatabaseEntityFrom(Application entity)
        {
            return entity.ToDatabase();
        }
    }
}
