using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.Sns;

namespace HousingRegisterApi.V1.Factories
{
    public interface ISnsFactory
    {
        ApplicationSns Create(Application application, string token);
    }
}
