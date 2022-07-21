using System;
using System.Collections.Generic;
using System.Linq;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Infrastructure;

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
                OtherMembers = domain.OtherMembers,
                Assessment = domain.Assessment,
                CalculatedBedroomNeed = domain.CalculatedBedroomNeed,
                ImportedFromLegacyDatabase = domain.ImportedFromLegacyDatabase,
            };
        }

        public static List<ApplicationResponse> ToResponse(this IEnumerable<Application> domainList)
        {
            if (null == domainList) return new List<ApplicationResponse>();
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }

        public static ApplicationSearchResultResponse ToResponse(this ApplicationSearchEntity searchEntity)
        {
            if (null == searchEntity) return null;
            return new ApplicationSearchResultResponse
            {
                ApplicationId = searchEntity.ApplicationId,
                AssignedTo = searchEntity.AssignedTo,
                BiddingNumber = searchEntity.BiddingNumber,
                CreatedAt = searchEntity.CreatedAt,
                DateOfBirth = searchEntity.DateOfBirth,
                FirstName = searchEntity.FirstName,
                HasAssessment = searchEntity.HasAssessment,
                MiddleName = searchEntity.MiddleName,
                NationalInsuranceNumber = searchEntity.NationalInsuranceNumber,
                Reference = searchEntity.Reference,
                SensitiveData = searchEntity.SensitiveData,
                Status = searchEntity.Status,
                SubmittedAt = searchEntity.SubmittedAt,
                Surname = searchEntity.Surname,
                Title = searchEntity.Title,
                OtherMembers = GetOtherMembers(searchEntity.OtherMembers)
            };
        }

        private static List<ApplicationOtherMemberSearchResultResponse> GetOtherMembers(List<ApplicationOtherMembersSearchEntity> otherMembers)
        {
            if (otherMembers == null) return null;
            List<ApplicationOtherMemberSearchResultResponse> response = new List<ApplicationOtherMemberSearchResultResponse>();

            foreach (var otherMember in otherMembers)
            {
                response.Add(new ApplicationOtherMemberSearchResultResponse
                {
                    DateOfBirth = otherMember.DateOfBirth,
                    FirstName = otherMember.FirstName,
                    Id = otherMember.Id,
                    MiddleName = otherMember.MiddleName,
                    NationalInsuranceNumber = otherMember.NationalInsuranceNumber,
                    Surname = otherMember.Surname
                });
            }
            return response;
        }
    }
}
