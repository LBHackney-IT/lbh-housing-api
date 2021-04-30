using System;
using System.Collections.Generic;
using HousingRegisterApi.V1.Domain;

namespace HousingRegisterApi.V1.Gateways
{
    public interface IApplicationApiGateway
    {
        Application GetApplicationById(Guid id);

        IEnumerable<Application> GetAll();
    }
}
