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
                {"resident_name", resident.Person.FirstName},
                {"verify_code", verifyCode},
            };
            var templateId = Environment.GetEnvironmentVariable("NOTIFY_TEMPLATE_VERIFY_CODE");
            return DeliverEmail(templateId, resident.ContactInformation.EmailAddress, personalisation);
        }

        private NotificationResponse DeliverEmail(string templateId, string emailAddress, Dictionary<string, object> personalisation)
        {
            return _client.SendEmail(emailAddress, templateId, personalisation, null, null);
        }
    }
}
