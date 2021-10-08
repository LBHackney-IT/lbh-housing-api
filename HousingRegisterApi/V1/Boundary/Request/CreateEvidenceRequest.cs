using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Boundary.Request
{
    public class CreateEvidenceRequest : CreateEvidenceRequestBase
    {
        public ResidentRequest Resident { get; set; }
        public List<string> DeliveryMethods { get; set; }
        public string Team { get; set; }
        public string Reason { get; set; }
        public string NotificationEmail { get; set; }
    }

    public class ResidentRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
