using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Boundary.Response
{
    public class EvidenceRequestResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public ResidentResponse Resident { get; set; }
        public List<string> DeliveryMethods { get; set; }
        public List<DocumentType> DocumentTypes { get; set; }
        public string Team { get; set; }
        public string Reason { get; set; }
        public string UserRequestedBy { get; set; }
        //public DocumentSubmission? DocumentSubmission { get; set; }
    }

    public class ResidentResponse
    {
        public Guid Id { get; set; }
        public string ReferenceId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class DocumentType
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
