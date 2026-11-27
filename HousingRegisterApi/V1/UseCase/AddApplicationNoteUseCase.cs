using Hackney.Core.Http;
using Hackney.Core.JWT;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Boundary.Response.Exceptions;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace HousingRegisterApi.V1.UseCase
{
    public class AddApplicationNoteUseCase : IAddApplicationNoteUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly IActivityGateway _activityGateway;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHttpContextWrapper _contextWrapper;
        private readonly ITokenFactory _tokenFactory;
        private readonly ApiOptions _apiOptions;

        public AddApplicationNoteUseCase(
            IApplicationApiGateway gateway,
            IActivityGateway applicationHistory,
            IHttpContextAccessor contextAccessor,
            IHttpContextWrapper contextWrapper,
            ITokenFactory tokenFactory,
            ApiOptions apiOptions)
        {
            _gateway = gateway;
            _activityGateway = applicationHistory;
            _contextAccessor = contextAccessor;
            _contextWrapper = contextWrapper;
            _tokenFactory = tokenFactory;
            _apiOptions = apiOptions;
        }

        public ApplicationResponse Execute(Guid id, AddApplicationNoteRequest request)
        {
            if (!StaffAuthValidator.IsStaffWithWriteAccess(
                _contextAccessor.HttpContext,
                _contextWrapper,
                _tokenFactory,
                _apiOptions))
            {
                throw new UnauthorizedStaffException("Staff authorization is required to add a note.");
            }

            var application = _gateway.GetApplicationById(id);

            if (application != null)
            {
                if (!String.IsNullOrWhiteSpace(request?.Note))
                {
                    _activityGateway.LogActivity(application,
                        new EntityActivity<ApplicationActivityType>(ApplicationActivityType.NoteAddedByUser, request.Note));
                }

                return application.ToResponse();
            }

            return null;
        }
    }
}
