using System;

namespace HousingRegisterApi.V1.Domain
{
    public class Address
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Postcode { get; set; }
        public string AddressType { get; set; }
    }
}
