using System;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Domain
{
    /// <summary>
    /// The application to the housing register.
    /// </summary>
    public class Application
    {
        public Guid Id { get; set; }

        /// <summary>
        /// A unique refrence for the application
        /// </summary>
        public string Reference { get; set; }

        // TODO: should this be a type?
        /// <summary>
        /// The current status of the application.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The date the application was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The date the application was submitted.
        /// </summary>
        public DateTime? SubmittedAt { get; set; }

        /// <summary>
        /// The main applicant associated with the application.
        /// </summary>
        public Applicant MainApplicant { get; set; }

        /// <summary>
        /// Other members involved with the application.
        /// </summary>
        public IEnumerable<Applicant> OtherMembers { get; set; }
    }
}
