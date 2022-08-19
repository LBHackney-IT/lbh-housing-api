using System;
using System.Collections.Generic;
using HousingRegisterApi.V1.Domain;

namespace HousingRegisterApi.V1.Infrastructure
{
    public interface IBiddingNumberGenerator
    {
        public string GetNextBiddingNumber(IEnumerable<Application> applications);

        bool IsExistingBiddingNumber(IEnumerable<Application> applications, Guid id, string biddingNumber);
    }
}
