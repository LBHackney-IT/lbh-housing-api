using HousingRegisterApi.V1.Domain;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public static class UseCaseTestsApplicationExtensions
    {
        public static void FixDates(this Application application)
        {
            application.MainApplicant.Person.DateOfBirth = application.MainApplicant.Person.DateOfBirth.Date;
            application.MainApplicant.MedicalNeed.FormRecieved = application.MainApplicant.MedicalNeed.FormRecieved.Date;
            application.MainApplicant.MedicalNeed.AssessmentDate = application.MainApplicant.MedicalNeed.AssessmentDate.Value.Date;
            application.Assessment.EffectiveDate = application.Assessment.EffectiveDate.Value.Date;
            application.Assessment.InformationReceivedDate = application.Assessment.InformationReceivedDate.Value.Date;

            foreach (var member in application.OtherMembers)
            {
                member.Person.DateOfBirth = member.Person.DateOfBirth.Date;
                member.MedicalNeed.AssessmentDate = member.MedicalNeed.AssessmentDate.Value.Date;
                member.MedicalNeed.FormRecieved = member.MedicalNeed.FormRecieved.Date;
            }
        }
    }
}
