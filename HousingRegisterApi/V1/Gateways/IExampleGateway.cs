using System.Collections.Generic;
using HousingRegisterApi.V1.Domain;

namespace HousingRegisterApi.V1.Gateways
{
    public interface IExampleGateway
    {
        Entity GetEntityById(int id);

        List<Entity> GetAll();
    }
}
