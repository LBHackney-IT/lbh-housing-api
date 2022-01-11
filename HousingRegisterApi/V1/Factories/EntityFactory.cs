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
                SensitiveData = databaseEntity.SensitiveData,
                AssignedTo = databaseEntity.AssignedTo,
                CreatedAt = databaseEntity.CreatedAt,
                SubmittedAt = databaseEntity.SubmittedAt,
                MainApplicant = databaseEntity.MainApplicant,
                OtherMembers = databaseEntity.OtherMembers,
                VerifyCode = databaseEntity.VerifyCode,
                VerifyExpiresAt = databaseEntity.VerifyExpiresAt,
                Assessment = databaseEntity.Assessment,
                CalculatedBedroomNeed = databaseEntity.CalculatedBedroomNeed,
                ImportedFromLegacyDatabase = databaseEntity.ImportedFromLegacyDatabase,
            };
        }

        public static ApplicationDbEntity ToDatabase(this Application entity)
        {
            return new ApplicationDbEntity
            {
                Id = entity.Id,
                Reference = entity.Reference,
                Status = entity.Status,
                SensitiveData = entity.SensitiveData,
                AssignedTo = entity.AssignedTo,
                CreatedAt = entity.CreatedAt,
                SubmittedAt = entity.SubmittedAt,
                MainApplicant = entity.MainApplicant,
                OtherMembers = entity.OtherMembers.ToList(),
                VerifyCode = entity.VerifyCode,
                VerifyExpiresAt = entity.VerifyExpiresAt,
                Assessment = entity.Assessment,
                CalculatedBedroomNeed = entity.CalculatedBedroomNeed,
                ImportedFromLegacyDatabase = entity.ImportedFromLegacyDatabase,
            };
        }
    }
}
