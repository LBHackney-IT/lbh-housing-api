namespace HousingRegisterApi.V1.Boundary.Request
{
    public class VerifyAuthRequest
    {
        public string Email { get; set; }

        public string Code { get; set; }
    }
}
