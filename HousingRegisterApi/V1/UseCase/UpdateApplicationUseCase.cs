using System;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Boundary.Response.Exceptions;
using HousingRegisterApi.V1.Factories;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.Infrastructure;
using HousingRegisterApi.V1.UseCase.Interfaces;

namespace HousingRegisterApi.V1.UseCase
{
    public class UpdateApplicationUseCase : IUpdateApplicationUseCase
    {
        private readonly IApplicationApiGateway _gateway;
        private readonly IBiddingNumberGenerator _biddingNumberGenerator;
        public UpdateApplicationUseCase(IApplicationApiGateway gateway, IBiddingNumberGenerator biddingNumberGenerator)
        {
            _gateway = gateway;
            _biddingNumberGenerator = biddingNumberGenerator;
        }

        public ApplicationResponse Execute(Guid id, UpdateApplicationRequest request)
        {
            if (request.Assessment != null
                && (request.Assessment.GenerateBiddingNumber || !string.IsNullOrEmpty(request.Assessment.BiddingNumber)))
            {
                var searchParameters = new SearchQueryParameter()
                {
                    HasAssessment = true
                };

                var applications = _gateway.GetApplications(searchParameters);
                if (request.Assessment.GenerateBiddingNumber)
                {
                    var biddingNumber = _biddingNumberGenerator.GetNextBiddingNumber(applications);
                    request.Assessment.BiddingNumber = biddingNumber;
                }
                else if (!string.IsNullOrEmpty(request.Assessment.BiddingNumber)
                    && _biddingNumberGenerator.IsExistingBiddingNumber(applications, id, request.Assessment.BiddingNumber))
                {
                    throw new DuplicateBiddingNumberException("Unable to update application with duplicate bidding number");
                }
            }

            return _gateway.UpdateApplication(id, request).ToResponse();
        }
    }
}
