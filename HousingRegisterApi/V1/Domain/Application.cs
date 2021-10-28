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

        /// <summary>
        /// Calculates the number of bedrooms required for the application
        /// </summary>
        /// <returns></returns>
        public int CalculateBedrooms()
        {
            if (this.MainApplicant == null || this.MainApplicant.Person == null)
            {
                throw new InvalidOperationException("Main applicant doesn't exist");
            }

            // combine all applicant into one collection
            var applicants = new List<Person> { this.MainApplicant.Person }.Concat(this.OtherMembers.Select(x => x.Person));

            // get a list of applicant's ages and gender
            var people = applicants.Select(x => new { x.Age, x.Gender });

            bool hasPartnerSharing = applicants.Any(x => !string.IsNullOrWhiteSpace(x.RelationshipType)
                && x.RelationshipType.Equals("partner", StringComparison.CurrentCultureIgnoreCase));

            // one bedroom each... except when there's a couple
            int over16Count = people.Where(x => x.Age >= 16).Count();
            int over16BedroomCount = hasPartnerSharing ? over16Count - 1 : over16Count;

            // 0.5 bedroom each.
            decimal under10BedroomCount = Decimal.Divide(people.Where(x => x.Age < 10).Count(), 2);

            // each child gender must have their own room
            var peopleBetween10And16 = people.Where(x => x.Age >= 10 && x.Age < 16).ToList();

            var over10PerGenderCount = peopleBetween10And16
                .GroupBy(x => x.Gender)
                .Select(gender =>
                {
                    int genderCount = gender.Count();

                    // when there's an uneven number of over 10s of a given gender, and an uneven number of under 10s in general...
                    if (genderCount % 2 != 0 && (under10BedroomCount * 2) % 2 != 0)
                    {
                        var matchedUnder10s = people.Where(x => x.Age < 10 && x.Gender == gender.Key).ToList();

                        // ...and at least one of the under 10s has this gender then they can share a room.
                        if (matchedUnder10s.Any())
                        {
                            // we can eliminate this over 10 from the count - they'll be added in the roundup of under 10s.
                            return new
                            {
                                Gender = gender.Key,
                                Count = Decimal.Divide((genderCount - 1), 2)
                            };
                        }
                    }

                    return new
                    {
                        Gender = gender.Key,
                        Count = Decimal.Divide(genderCount, 2)
                    };

                }).ToList();

            decimal over10PerGenderBedroomCount = over10PerGenderCount.Select(x => x.Count).Sum(x => Math.Ceiling(x));

            var total = over16BedroomCount + Math.Ceiling(under10BedroomCount) + over10PerGenderBedroomCount;

            return Convert.ToInt32(total);
        }

        /// <summary>
        /// Recalculates the number of bedrooms and assigns it to the assessment
        /// </summary>
        public void RecalulateBedrooms()
        {
            if (this.Assessment == null)
            {
                throw new InvalidOperationException("An assessment doesn't exist");
            }

            int bedroomNeed = this.CalculateBedrooms();
            Assessment.BedroomNeed = bedroomNeed;
        }
    }
}
