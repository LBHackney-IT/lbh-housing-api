using System;
using System.Collections.Generic;
using System.Linq;
using HousingRegisterApi.V1.Boundary.Response;
using HousingRegisterApi.V1.Domain;

namespace HousingRegisterApi.V1.Factories
{
    public static class ResponseFactory
    {
        //TODO: Map the fields in the domain object(s) to fields in the response object(s).
        // More information on this can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/Factory-object-mappings
        public static ResponseObject ToResponse(this Entity domain)
        {
            if (null == domain) return null;
            return new ResponseObject
            {
                Id = domain.Id,
                Name = domain.Name,
                CreatedAt = domain.CreatedAt.FormatDate()
            };
        }

        public static List<ResponseObject> ToResponse(this IEnumerable<Entity> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }

        public static string FormatDate(this DateTime dob)
        {
            return dob.ToString("yyyy-MM-dd");
        }
    }
}
