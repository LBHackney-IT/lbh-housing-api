using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using System;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Gateways
{
    public interface IApplicationApiGateway
    {
        IEnumerable<Application> GetAll();

        IEnumerable<Application> GetAllBySearchTerm(SearchApplicationRequest searchParameters);

        Application GetApplicationById(Guid id);

        Application CreateNewApplication(CreateApplicationRequest request);

        Application UpdateApplication(Guid id, UpdateApplicationRequest request);

        Application CompleteApplication(Guid id);
    }
}
