using System.Collections.Generic;

namespace HousingRegisterApi.V1.Boundary.Request
{
    public class CreateEvidenceRequestBase
    {
        public CreateEvidenceRequestBase()
        {
            DocumentTypes = new List<string>();
        }

        public List<string> DocumentTypes { get; set; }
        public string UserRequestedBy { get; set; }
    }
}
