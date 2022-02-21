using Hackney.Core.DynamoDb;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Gateways
{
    public interface IApplicationApiGateway
    {
        IEnumerable<Application> GetApplications(SearchQueryParameter searchParameters);
        Task<(IEnumerable<Application>, string)> GetAllApplicationsAsync(SearchQueryParameter searchParameters);

        Task<(IEnumerable<Application>, string)> GetApplicationsByStatusAsync(SearchQueryParameter searchParameters);
        Task<(IEnumerable<Application>, string)> GetApplicationsAsync(SearchQueryParameter searchParameters);

        Task<(IEnumerable<Application>, string)> GetApplicationsByAssignedToAsync(SearchQueryParameter searchParameters);

        IEnumerable<Application> GetApplicationsAtStatus(params string[] status);

        Application GetApplicationById(Guid id);

        Application CreateNewApplication(CreateApplicationRequest request);

        Application UpdateApplication(Guid id, UpdateApplicationRequest request);

        Application CompleteApplication(Guid id);

        Application CreateVerifyCode(Guid id, CreateAuthRequest request);

        Application ConfirmVerifyCode(VerifyAuthRequest request);

        Application GetIncompleteApplication(string email);

        Application ImportApplication(ImportApplicationRequest request);
    }
}
