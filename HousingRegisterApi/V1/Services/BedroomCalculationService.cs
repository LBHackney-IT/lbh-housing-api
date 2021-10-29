using HousingRegisterApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Services
{
    public static class BedroomCalculationService
    {
        public static int Calculate(Application application)
        {
            if (application.MainApplicant == null || application.MainApplicant.Person == null)
            {
                throw new InvalidOperationException("Main applicant doesn't exist");
            }

            // combine all applicant into one collection
            var applicants = new List<Person> { application.MainApplicant.Person }.Concat(application.OtherMembers.Select(x => x.Person));

            // get a list of applicant's ages and gender
            var people = applicants.Select(x => new { Age = x.Age, Gender = x.Gender });

            bool hasPartnerSharing = applicants.Any(x => x.RelationshipType.Equals("partner", StringComparison.CurrentCultureIgnoreCase));

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
    }
}
