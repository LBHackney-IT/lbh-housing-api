using System;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class VerifyCodeGenerator : IVerifyCodeGenerator
    {
        public string GenerateCode()
        {
            var generator = new Random();
            return generator.Next(0, 1000000).ToString("D6");
        }
    }
}
