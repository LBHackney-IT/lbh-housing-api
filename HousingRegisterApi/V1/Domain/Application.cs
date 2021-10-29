using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// A verify code for the application
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// The date which the verify code expires
        /// </summary>
        public DateTime? VerifyExpiresAt { get; set; }

        /// <summary>
        /// The officer who this application is assigned to.
        /// e.g. officer@hackney.gov.uk
        /// </summary>
        public string AssignedTo { get; set; }

        /// <summary>
        /// Does the application contain sensitive data
        /// </summary>
        public bool SensitiveData { get; set; }

        /// <summary>
        /// Populated after assessment.
        /// </summary>
        public Assessment Assessment { get; set; }      
    }
}
