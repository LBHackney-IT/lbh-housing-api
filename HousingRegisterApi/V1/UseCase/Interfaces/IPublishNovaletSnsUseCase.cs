using Amazon.SimpleNotificationService.Model;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface IPublishNovaletSnsUseCase
    {
        Task<PublishResponse> Execute();
    }
}
