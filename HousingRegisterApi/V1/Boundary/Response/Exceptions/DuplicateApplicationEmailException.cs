using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Boundary.Response.Exceptions
{
    public class DuplicateApplicationEmailException : Exception
    {
        public IReadOnlyList<Guid> ApplicationIds { get; }

        public DuplicateApplicationEmailException(IEnumerable<Guid> applicationIds)
            : base($"An application already exists for this email. Case IDs: {string.Join(",", applicationIds)}")
        {
            ApplicationIds = applicationIds.ToList();
        }
    }
}
