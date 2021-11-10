using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways.Interfaces;

namespace HousingRegisterApi.V1.Gateways
{
    public class ApplicationAuditHistory : IAuditHistory
    {
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public ApplicationAuditHistory(           
            ISnsGateway snsGateway,
            ISnsFactory snsFactory)
        {
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public void Audit(Application application)
        {
            _snsGateway.Publish(_snsFactory.Create(application));
        }
    }
}
