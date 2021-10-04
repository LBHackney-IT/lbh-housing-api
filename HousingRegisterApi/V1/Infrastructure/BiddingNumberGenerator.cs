using System.Collections.Generic;
using System.Linq;
using HousingRegisterApi.V1.Domain;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class BiddingNumberGenerator : IBiddingNumberGenerator
    {
        /// <summary>
        /// This is a legacy value, based on initial data.
        /// </summary>
        private static double LegacyBiddingNumber => 2158458;

        public string GetNextBiddingNumber(IEnumerable<Application> applications)
        {
            var biddingNumbers = applications
                .Where(x => !string.IsNullOrEmpty(x.Assessment.BiddingNumber))
                .Select(x => x.Assessment.BiddingNumber)
                .OrderByDescending(x => x)
                .ToList();

            if (biddingNumbers.Count == 0)
                return LegacyBiddingNumber.ToString();

            _ = double.TryParse(biddingNumbers.First(), out double highestBiddingNumber);
            var nextBiddingNumber = highestBiddingNumber + 1;

            if (nextBiddingNumber < LegacyBiddingNumber)
                nextBiddingNumber = LegacyBiddingNumber + 1;

            return nextBiddingNumber.ToString();
        }
    }
}
