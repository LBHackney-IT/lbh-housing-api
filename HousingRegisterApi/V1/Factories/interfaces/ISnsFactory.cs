using Hackney.Core.JWT;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Domain.Sns;

namespace HousingRegisterApi.V1.Factories
{
    public interface ISnsFactory
    {
        ApplicationSns Create(Application application);

        ApplicationSns Update(Application application);
    }
}
