namespace HousingRegisterApi.V1.Boundary.Request
{
    public class SearchApplicationRequest : PaginatedDataRequestBase
    {
        public string Reference { get; set; }

        public string Surname { get; set; }

        public string NationalInsurance { get; set; }

        public string Status { get; set; }

        public string AssignedTo { get; set; }

        public string OrderBy { get; set; }
    }
}
