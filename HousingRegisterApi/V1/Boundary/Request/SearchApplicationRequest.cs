namespace HousingRegisterApi.V1.Boundary.Request
{
    public class SearchApplicationRequest : PaginatedDataRequestBase
    {
        public string ReferenceNumber { get; set; }

        public string Surname { get; set; }

        public string NationalInsuranceNumber { get; set; }

        public string Status { get; set; }

        public string OrderBy { get; set; }
    }
}
