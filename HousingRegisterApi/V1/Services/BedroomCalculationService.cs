using HousingRegisterApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Services
{
    public static class BedroomCalculationService
    {
        //public static int CalculateOrig(Application application)
        //{
        //    if (application.MainApplicant == null || application.MainApplicant.Person == null)
        //    {
        //        throw new InvalidOperationException("Main applicant doesn't exist");
        //    }

        //    // combine all applicant into one collection
        //    var applicants = new List<Person> { application.MainApplicant.Person }.Concat(application.OtherMembers.Select(x => x.Person));

        //    // get a list of applicant's ages and gender
        //    var people = applicants.Select(x => new { Age = x.Age, Gender = x.Gender });

        //    bool hasPartnerSharing = applicants.Any(x => x.RelationshipType.Equals("partner", StringComparison.CurrentCultureIgnoreCase));

        //    // one bedroom each... except when there's a couple
        //    int over21Count = people.Where(x => x.Age >= 21).Count();
        //    int over21BedroomCount = hasPartnerSharing ? over21Count - 1 : over21Count;

        //    // 0.5 bedroom each.
        //    decimal under10BedroomCount = Decimal.Divide(people.Where(x => x.Age < 10).Count(), 2);

        //    // each child gender must have their own room
        //    var peopleBetween10And21 = people.Where(x => x.Age >= 10 && x.Age < 21).ToList();

        //    var over10PerGenderCount = peopleBetween10And21
        //        .GroupBy(x => x.Gender)
        //        .Select(gender =>
        //        {
        //            int genderCount = gender.Count();

        //            // when there's an uneven number of over 10s of a given gender, and an uneven number of under 10s in general...
        //            if (genderCount % 2 != 0 && (under10BedroomCount * 2) % 2 != 0)
        //            {
        //                var matchedUnder10s = people.Where(x => x.Age < 10 && x.Gender == gender.Key).ToList();

        //                // ...and at least one of the under 10s has this gender then they can share a room.
        //                if (matchedUnder10s.Any())
        //                {
        //                    // we can eliminate this over 10 from the count - they'll be added in the roundup of under 10s.
        //                    return new
        //                    {
        //                        Gender = gender.Key,
        //                        Count = Decimal.Divide((genderCount - 1), 2)
        //                    };
        //                }
        //            }

        //            return new
        //            {
        //                Gender = gender.Key,
        //                Count = Decimal.Divide(genderCount, 2)
        //            };

        //        }).ToList();

        //    decimal over10PerGenderBedroomCount = over10PerGenderCount.Select(x => x.Count).Sum(x => Math.Ceiling(x));

        //    var total = over21BedroomCount + Math.Ceiling(under10BedroomCount) + over10PerGenderBedroomCount;

        //    return Convert.ToInt32(total);
        //}       

        public static int Calculate(Application application)
        {
            // combine all applicants into one collection
            var people = new List<Person> { application.MainApplicant.Person }
                .Concat(application.OtherMembers.Select(x => x.Person))
                .ToList();

            // create a list of available occupants, we will use this to remove occupants that have been allocated rooms
            var unoccupants = people.Select(x => new Occupant(x.Age, x.Gender, x.RelationshipType)).ToList();

            // rules must be applied in order
            var count = 0;
            count += ApplyRule1(unoccupants);
            count += ApplyRule2(unoccupants);
            count += ApplyRule3(unoccupants);
            count += ApplyRule4(unoccupants);
            count += ApplyRule5(unoccupants);

            return count;
        }

        /// <summary>
        /// One room for each couple who are married, in a civil partnership or otherwise cohabiting who live in your household
        /// </summary>
        /// <param name="unoccupants"></param>
        /// <returns></returns>
        private static int ApplyRule1(List<Occupant> unoccupants)
        {
            var predicate = new Predicate<Occupant>(x => string.IsNullOrWhiteSpace(x.RelationshipType) || x.RelationshipType == "partner");
            var rule = unoccupants.FindAll(predicate);
            // pair up and return biggest int
            var ruleCount = Convert.ToInt32(Math.Ceiling(Decimal.Divide(rule.Count, 2)));
            unoccupants.RemoveAll(predicate);
            return ruleCount;
        }

        /// <summary>
        /// One room for each single person over 21 in your household
        /// </summary>
        /// <param name="unoccupants"></param>
        /// <returns></returns>
        private static int ApplyRule2(List<Occupant> unoccupants)
        {
            var predicate = new Predicate<Occupant>(x => x.Age >= 21);
            var rule = unoccupants.FindAll(predicate);
            var ruleCount = rule.Count;
            unoccupants.RemoveAll(predicate);
            return ruleCount;
        }

        /// <summary>
        /// One room for every two persons under 21 of the same sex in your household
        /// </summary>
        /// <param name="unoccupants"></param>
        /// <returns></returns>
        private static int ApplyRule3(List<Occupant> unoccupants)
        {
            // at this point only people under 21 should only exist in the household
            var ruleCount = 0;
            var under21ByGender = unoccupants.GroupBy(x => x.Gender);

            foreach (var under21ByGenderGroup in under21ByGender)
            {
                var peopleInGroup = under21ByGenderGroup.ToList();

                // loop though while only pairs exist
                while (CanPair(peopleInGroup))
                {
                    // take top 2
                    var occupant1 = peopleInGroup.Last();
                    peopleInGroup.RemoveAt(peopleInGroup.Count - 1);
                    unoccupants.RemoveAll(x => x.Id == occupant1.Id);

                    var occupant2 = peopleInGroup.Last();
                    peopleInGroup.RemoveAt(peopleInGroup.Count - 1);
                    unoccupants.RemoveAll(x => x.Id == occupant2.Id);

                    ruleCount++;
                }
            }

            return ruleCount;
        }

        /// <summary>
        /// One room for every two children of opposite sexes, provided each is under the age of 10 in your household
        /// </summary>
        /// <param name="unoccupants"></param>
        /// <returns></returns>
        private static int ApplyRule4(List<Occupant> unoccupants)
        {
            var ruleCount = 0;
            var childrenUnder10 = unoccupants.FindAll(x => x.Age < 10);

            // loop though while only pairs exist
            while (CanPair(childrenUnder10))
            {
                // take top 2
                var child1 = childrenUnder10.Last();
                childrenUnder10.RemoveAt(childrenUnder10.Count - 1);
                unoccupants.RemoveAll(x => x.Id == child1.Id);

                var child2 = childrenUnder10.Last();
                childrenUnder10.RemoveAt(childrenUnder10.Count - 1);
                unoccupants.RemoveAll(x => x.Id == child2.Id);

                ruleCount++;
            }

            return ruleCount;
        }

        /// <summary>
        /// One room for every single person of any age, including an adult child, of either sex in your household
        /// when there is no-one else in the household suitable to share with.
        /// </summary>
        /// <param name="unoccupants"></param>
        /// <returns></returns>
        private static int ApplyRule5(List<Occupant> unoccupants)
        {
            var ruleCount = unoccupants.Count;
            unoccupants.Clear();
            return ruleCount;
        }

        /// <summary>
        /// check if we can create pairs of occupants
        /// </summary>
        /// <param name="occupants"></param>
        /// <returns></returns>
        private static bool CanPair(List<Occupant> occupants)
        {
            return occupants.Count >= 2 || (occupants.Any() && occupants.Count % 2 == 0);
        }

        /// <summary>
        /// Houshold Occupant
        /// </summary>
        private class Occupant
        {
            public Guid Id { get; }

            public int Age { get; }

            public string Gender { get; }

            public string RelationshipType { get; }

            public Occupant(int age, string gender, string relationToMainApplicant)
            {
                Id = Guid.NewGuid();
                Age = age;
                Gender = gender;
                RelationshipType = relationToMainApplicant;
            }
        }
    }
}
