using System;
using System.Collections.Generic;
using HousingRegisterApi.V1.Domain;
using Notify.Interfaces;
using Notify.Models.Responses;

namespace HousingRegisterApi.V1.Gateways
{
    public class NotifyGateway : INotifyGateway
    {
        private readonly INotificationClient _client;

        public NotifyGateway(INotificationClient client)
        {
            _client = client;
        }

        public NotificationResponse SendVerifyCode(Applicant resident, string verifyCode)
        {
            var personalisation = new Dictionary<string, object>
            {
                {"resident_name", ""},
                {"verify_code", verifyCode},
            };
            var templateId = Environment.GetEnvironmentVariable("NOTIFY_TEMPLATE_VERIFY_CODE");
            return DeliverEmail(templateId, resident.ContactInformation.EmailAddress, personalisation);
        }

        public NotificationResponse NotifyResidentOfBedroomChange(string email, string name, int currentBedroomNeed, string currentBand)
        {
            var personalisation = new Dictionary<string, object>
            {
                {"resident", name},
                {"bedroom_need", currentBedroomNeed},
                {"band", currentBand},
            };

            var templateId = Environment.GetEnvironmentVariable("NOTIFY_TEMPLATE_BEDROOMCANGE");
            _client.SendEmail(email, templateId, personalisation, null, null);
            return new NotificationResponse();
        }

        private NotificationResponse DeliverEmail(string templateId, string emailAddress, Dictionary<string, object> personalisation)
        {
            return _client.SendEmail(emailAddress, templateId, personalisation, null, null);
        }
    }
}
