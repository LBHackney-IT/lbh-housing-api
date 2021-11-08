using System;

namespace HousingRegisterApi.V1.Domain
{
    public class Assessment
    {
        public DateTime? EffectiveDate { get; set; }

        public DateTime? InformationReceivedDate { get; set; }

        public int? BedroomNeed { get; set; }

        public string Band { get; set; }

        public string Reason { get; set; }

        public string BiddingNumber { get; set; }

        public bool GenerateBiddingNumber { get; set; }
    }
}
