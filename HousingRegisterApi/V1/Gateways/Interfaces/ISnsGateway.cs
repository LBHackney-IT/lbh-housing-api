using HousingRegisterApi.V1.Domain.Sns;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Gateways
{
    public interface ISnsGateway
    {
        Task Publish(ApplicationSns applicationSns);
    }
}
