using System;

namespace HousingRegisterApi.V1.Infrastructure
{
    public interface ITokenGenerator
    {
        public string GenerateTokenForApplication(Guid id);
    }
}
