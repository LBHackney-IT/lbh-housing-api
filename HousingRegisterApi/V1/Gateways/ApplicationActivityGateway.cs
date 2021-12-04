using Hackney.Core.DynamoDb;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Hackney.Shared.ActivityHistory.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Gateways
{
    public class ApplicationActivityGateway : IActivityGateway
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHttpContextWrapper _contextWrapper;
        private readonly ITokenFactory _tokenFactory;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;
        private readonly ILogger<ApplicationActivityGateway> _logger;

        public ApplicationActivityGateway(
            HttpClient httpClient,
            ApiOptions apiOptions,
            IHttpContextAccessor contextAccessor,
            IHttpContextWrapper contextWrapper,
            ITokenFactory tokenFactory,
            ISnsGateway snsGateway,
            ISnsFactory snsFactory,
            ILogger<ApplicationActivityGateway> logger)
        {
            _client = httpClient;
            _contextAccessor = contextAccessor;
            _contextWrapper = contextWrapper;
            _tokenFactory = tokenFactory;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
            _logger = logger;

            _client.BaseAddress = apiOptions.ActivityHistoryApiUrl;
        }

        public void LogActivity(Application application, EntityActivity<ApplicationActivityType> activity)
        {
            // we only want to log activites after an application has been submitted
            if (activity == null
                || application == null
                || application.Status == ApplicationStatus.Verification
                || application.Status == ApplicationStatus.New)
            {
                return;
            }

            var token = GetToken(application, activity);

            if (token != null && activity.HasChanges())
            {
                var applicationSnsMessage = _snsFactory.Update(application.Id, activity.OldData, activity.NewData, token);
                _snsGateway.Publish(applicationSnsMessage);
            }
            else if (token == null)
            {
                _logger.LogWarning("Unable to publish activity. No valid auth token has been found");
            }
        }

        public async Task<List<ActivityHistoryResponseObject>> GetActivities(Guid applicationId)
        {
            var result = new List<ActivityHistoryResponseObject>();

            try
            {
                var uri = new Uri($"api/v1/activityhistory?targetId={applicationId}&pageSize=500", UriKind.Relative);
                _client.DefaultRequestHeaders.Add("Authorization", GetAuthorizationHeader());

                var response = await _client.GetAsync(uri).ConfigureAwait(true);
                _logger.LogInformation("activity gateway response:" + applicationId + " " + response.StatusCode);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var pagedResult = await response.Content.ReadAsAsync<PagedResult<ActivityHistoryResponseObject>>().ConfigureAwait(true);
                    result = pagedResult.Results;

                    _logger.LogInformation("activity gateway result count " + result.Count);
                }
                else if (response.StatusCode != HttpStatusCode.NotFound)
                {
                    _logger.LogError("Error calling activity gateway");
                }
            }
            catch (Exception exp)
            {
                _logger.LogError($"Error calling activity gateway: {exp.Message}");
            }

            return result;
        }

        private Token GetToken(Application application, EntityActivity<ApplicationActivityType> activity)
        {
            var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(_contextAccessor.HttpContext));

            // residents will not have an auth token so
            // generate a simple token to hold some user info
            if (ActivityPerformedByResident(activity) == true || token == null)
            {
                token = new Token()
                {
                    Name = application.MainApplicant.Person.FullName,
                    Email = application.MainApplicant.ContactInformation.EmailAddress,
                };
            }

            return token;
        }

        private string GetAuthorizationHeader()
        {
            return _contextAccessor.HttpContext.Request.Headers["Authorization"];
        }

        private static bool ActivityPerformedByResident(EntityActivity<ApplicationActivityType> activity)
        {
            return activity.ActivityType == ApplicationActivityType.SubmittedByResident;
        }
    }
}
