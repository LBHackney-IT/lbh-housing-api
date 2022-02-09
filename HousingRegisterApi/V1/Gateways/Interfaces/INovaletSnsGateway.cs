using Amazon.SimpleNotificationService.Model;
using HousingRegisterApi.V1.Domain.Sns;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Gateways
{
    public interface INovaletSnsGateway
    {
        Task<PublishResponse> Publish(NovaletSns novaletSns);
    }
}
