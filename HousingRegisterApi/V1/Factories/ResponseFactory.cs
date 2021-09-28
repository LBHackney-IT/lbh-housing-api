using System.Collections.Generic;
using System.Linq;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;

namespace HousingRegisterApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static ApplicationResponse ToResponse(this Application domain)
        {
            if (null == domain) return null;
            return new ApplicationResponse
            {
                Id = domain.Id,
                Reference = domain.Reference,
                Status = domain.Status,
                SensitiveData = domain.SensitiveData,
                AssignedTo = domain.AssignedTo,
                CreatedAt = domain.CreatedAt,
                SubmittedAt = domain.SubmittedAt,
                MainApplicant = domain.MainApplicant,
                OtherMembers = domain.OtherMembers
            };
        }

        public static List<ApplicationResponse> ToResponse(this IEnumerable<Application> domainList)
        {
            if (null == domainList) return new List<ApplicationResponse>();
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
