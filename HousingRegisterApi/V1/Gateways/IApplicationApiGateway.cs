using System;
using System.Collections.Generic;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;

namespace HousingRegisterApi.V1.Gateways
{
    public interface IApplicationApiGateway
    {
        IEnumerable<Application> GetAll();

        Application GetApplicationById(Guid id);

        Application CreateNewApplication(CreateApplicationRequest request);

        Application UpdateApplication(Guid id, UpdateApplicationRequest request);
    }
}
