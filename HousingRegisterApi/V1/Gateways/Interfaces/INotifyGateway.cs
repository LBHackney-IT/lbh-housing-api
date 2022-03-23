using HousingRegisterApi.V1.Domain;
using Notify.Models.Responses;

namespace HousingRegisterApi.V1.Gateways
{
    public interface INotifyGateway
    {
        public NotificationResponse SendVerifyCode(Applicant resident, string verifyCode);

        public NotificationResponse NotifyResidentOfBedroomChange(string email, string name, int currentBedroomNeed, string currentBand);
    }
}
