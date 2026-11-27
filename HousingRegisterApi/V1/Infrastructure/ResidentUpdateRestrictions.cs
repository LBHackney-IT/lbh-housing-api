using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Infrastructure
{
    /// <summary>
    /// Limits what a resident's update can change.
    ///
    /// The resident app sends whole MainApplicant and OtherMembers objects, and the gateway
    /// replaces the stored ones. The copy a resident holds can be out of date, and parts of
    /// those objects (the medical need) are set by staff, so the request is merged onto the
    /// stored application instead of replacing it.
    /// </summary>
    public static class ResidentUpdateRestrictions
    {
        // Statuses in which an application is still being completed. Only then can a resident
        // add or remove household members or disqualify their own application.
        private static readonly string[] _draftStatuses =
        {
            ApplicationStatus.New,
            ApplicationStatus.ManualDraft,
        };

        /// <summary>
        /// Rewrites the request in place so it only holds changes a resident may make.
        /// </summary>
        /// <param name="request">The update sent by the resident</param>
        /// <param name="stored">The application as currently stored</param>
        public static void Apply(UpdateApplicationRequest request, Application stored)
        {
            if (request == null)
            {
                return;
            }

            // Owned by staff and the system
            request.SensitiveData = null;
            request.AssignedTo = null;
            request.Assessment = null;

            var storedIsDraft = stored != null && _draftStatuses.Contains(stored.Status);

            // The resident app disqualifies an application when the eligibility check fails
            var isDisqualifyingADraft =
                string.Equals(request.Status, ApplicationStatus.Disqualified, StringComparison.Ordinal)
                && storedIsDraft;

            if (!isDisqualifyingADraft)
            {
                request.Status = null;
            }

            if (request.MainApplicant != null)
            {
                request.MainApplicant = MergeApplicant(stored?.MainApplicant, request.MainApplicant);
            }

            if (request.OtherMembers != null)
            {
                request.OtherMembers = MergeHousehold(stored?.OtherMembers, request.OtherMembers, storedIsDraft);
            }
        }

        /// <summary>
        /// Takes the details a resident owns from the request and everything else from the stored applicant.
        /// </summary>
        private static Applicant MergeApplicant(Applicant stored, Applicant requested)
        {
            return new Applicant
            {
                Person = requested.Person ?? stored?.Person,
                Address = requested.Address ?? stored?.Address,
                ContactInformation = requested.ContactInformation ?? stored?.ContactInformation,
                Questions = requested.Questions ?? stored?.Questions,

                // Set by staff, never by the resident
                MedicalNeed = stored?.MedicalNeed,
            };
        }

        private static List<Applicant> MergeHousehold(
            IEnumerable<Applicant> stored,
            IEnumerable<Applicant> requested,
            bool membershipCanChange)
        {
            var storedMembers = (stored ?? Enumerable.Empty<Applicant>()).Where(x => x != null).ToList();
            var requestedMembers = requested.Where(x => x != null).ToList();

            if (membershipCanChange)
            {
                // The resident's list is the household: members they left out are removed and
                // new ones are added. Matching members keep the staff-owned details.
                return requestedMembers
                    .Select(member => MergeApplicant(storedMembers.FirstOrDefault(s => IsSamePerson(s, member)), member))
                    .ToList();
            }

            // Once the application is past draft the household belongs to staff. Existing members
            // can have their own details updated, but nobody is added or removed.
            return storedMembers
                .Select(member =>
                {
                    var update = requestedMembers.FirstOrDefault(r => IsSamePerson(member, r));
                    return update == null ? member : MergeApplicant(member, update);
                })
                .ToList();
        }

        private static bool IsSamePerson(Applicant a, Applicant b)
        {
            return a?.Person != null
                && b?.Person != null
                && a.Person.Id != Guid.Empty
                && a.Person.Id == b.Person.Id;
        }
    }
}
