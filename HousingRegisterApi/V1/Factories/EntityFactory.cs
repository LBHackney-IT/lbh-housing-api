using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Infrastructure;
using System.Linq;

namespace HousingRegisterApi.V1.Factories
{
    public static class EntityFactory
    {
        public static Application ToDomain(this ApplicationDbEntity databaseEntity)
        {
            return new Application
            {
                Id = databaseEntity.Id,
                Reference = databaseEntity.Reference,
                Status = databaseEntity.Status,
                CreatedAt = databaseEntity.CreatedAt,
                MainApplicant = databaseEntity.MainApplicant,
                OtherMembers = databaseEntity.OtherMembers
            };
        }

        public static ApplicationDbEntity ToDatabase(this Application entity)
        {
            return new ApplicationDbEntity
            {
                Id = entity.Id,
                Reference = entity.Reference,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                MainApplicant = entity.MainApplicant,
                OtherMembers = entity.OtherMembers.ToList()
            };
        }
    }
}
