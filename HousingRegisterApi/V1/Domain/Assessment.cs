using System;

namespace HousingRegisterApi.V1.Domain
{
    public class Assessment
    {
        public DateTime EffectiveDate { get; set; }

        public string Band { get; set; }

        public string BiddingNumber { get; set; }
    }
}
