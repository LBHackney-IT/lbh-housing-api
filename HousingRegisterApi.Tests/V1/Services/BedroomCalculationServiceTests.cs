using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.Tests.V1.Services
{
    [TestFixture]
    public class BedroomCalculationServiceTests
    {
        private const string Male = "M";
        private const string Female = "F";
        private const string MainApplicant = "";
        private const string MainApplicantIsPartner = "partner";
        private const string MainApplicantIsMyParent = "parent";

        [Test(Description = "Single parent with children under the age of 10 of different genders returns 2 bedrooms")]
        public void SingleParentWithChildrenUnder10OfDifferentGendersReturns2Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
                new Tuple<int, string, string>(25, Male, MainApplicant),
                new Tuple<int, string, string>(5, Female, MainApplicantIsMyParent),
                new Tuple<int, string, string>(8, Male, MainApplicantIsMyParent),
            });

            // assert
            AssertBedrooms(2, application);
        }

        [Test(Description = "Single parent with children of different genders where one is over 10 returns 3 bedrooms")]
        public void SingleParentWithChildrenOfDifferentGendersWhereOneIsOver10ShouldBeAwarded2Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
                new Tuple<int, string, string>(25, Male, MainApplicant),
                new Tuple<int, string, string>(5, Male, MainApplicantIsMyParent),
                new Tuple<int, string, string>(10, Female, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Single parent with children of ages between 5 to 15 of different genders returns 3 bedrooms")]
        public void SingleParentWithChildrenOfAgesBetween5To15OfDifferentGendersReturns3Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
                new Tuple<int, string, string>(25, Male, MainApplicant),
                new Tuple<int, string, string>(15, Male, MainApplicantIsMyParent),
                new Tuple<int, string, string>(10, Female, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Family with 2 boys aged between 5 and 15 and 2 girls aged 10 and 12 should return 3 bedrooms")]
        public void FamilyWith2BoysAgedBetween5And15And2GirlsAged10And12ShouldBeAwarded2Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
                new Tuple<int, string, string>(35, Male, MainApplicant),
                new Tuple<int, string, string>(30, Female, MainApplicantIsPartner),
                new Tuple<int, string, string>(5, Male, MainApplicantIsMyParent),
                new Tuple<int, string, string>(15, Male, MainApplicantIsMyParent),
                new Tuple<int, string, string>(10, Female, MainApplicantIsMyParent),
                new Tuple<int, string, string>(12, Female, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Family with 2 boys aged between 5 and 15 and 2 girls aged 10 and 12 should return 3 bedrooms")]
        public void UnorderedFamilyWith2BoysAgedBetween5And15And2GirlsAged10And12ShouldBeAwarded2Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
                new Tuple<int, string, string>(30, Female, MainApplicantIsPartner),
                new Tuple<int, string, string>(10, Female, MainApplicantIsMyParent),
                new Tuple<int, string, string>(5, Male, MainApplicantIsMyParent),
                new Tuple<int, string, string>(35, Male, MainApplicant),
                new Tuple<int, string, string>(12, Female, MainApplicantIsMyParent),
                new Tuple<int, string, string>(15, Male, MainApplicantIsMyParent),
            });

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Couple without children returns 1 bedroom")]
        public void CoupleWithoutChildrenReturns1Bedroom()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
                new Tuple<int, string, string>(30, Male, MainApplicant),
                new Tuple<int, string, string>(25, Female, MainApplicantIsPartner)
            });

            // assert
            AssertBedrooms(1, application);
        }

        [Test(Description = "Family with children under the age of 10 returns 2 bedrooms")]
        public void FamilyWithChildrenUnderTheAgeOf10WithDifferentGendersReturns2Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
                new Tuple<int, string, string>(30, Male, MainApplicant),
                new Tuple<int, string, string>(25, Female, MainApplicantIsPartner),
                new Tuple<int, string, string>(5, Male, MainApplicantIsMyParent),
                new Tuple<int, string, string>(7, Female, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(2, application);
        }

        [Test(Description = "Family with children over the age of 10 of different genders returns 3 bedrooms")]
        public void FamilyWithChildrenOverTheAgeOf10WithDifferentGendersReturns3Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
                new Tuple<int, string, string>(30, Male, MainApplicant),
                new Tuple<int, string, string>(25, Female, MainApplicantIsPartner),
                new Tuple<int, string, string>(11, Male, MainApplicantIsMyParent),
                new Tuple<int, string, string>(12, Female, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(3, application);
        }

        //[Test(Description = "Single person under 35yrs returns 0 bedrooms")]
        //public void SinglePersonUnder35yrsReturns0Bedrooms()
        //{
        //    // arrange            
        //    Application application = CreateApplication(new List<Tuple<int, string, string>>()
        //    {
        //      new Tuple<int, string, string>(25, Male, MainApplicant)
        //    });

        //    // assert
        //    AssertBedrooms(0, application);
        //}

        [Test(Description = "Couple returns 1 bedroom")]
        public void CoupleReturns1Bedroom()
        {
            // arrange            
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(25, Male, MainApplicant),
              new Tuple<int, string, string>(25, Female, MainApplicantIsPartner)
            });

            // assert
            AssertBedrooms(1, application);
        }

        [Test(Description = "Single person 35yrs Plus returns 1 bedroom")]
        public void SinglePerson35yrsPlusReturns1Bedroom()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(35, Male, MainApplicant)
            });

            // assert
            AssertBedrooms(1, application);
        }

        [Test(Description = "Single person returns 1 bedrooms")]
        public void SinglePersonReturns1Bedroom()
        {
            // arrange            
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(25, Male, MainApplicant)
            });

            // assert
            AssertBedrooms(1, application);
        }

        [Test(Description = "Family with 1 child returns 2 bedrooms")]
        public void FamilyWith1ChildReturns2Bedrooms()
        {
            // arrange            
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(25, Male, MainApplicant),
              new Tuple<int, string, string>(25, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(5, Male, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(2, application);
        }

        [Test(Description = "Family with 2 children of the same sex under 21yrs returns 2 bedrooms")]
        public void FamilyWith2ChildrenOfTheSameSexUnder21yrsReturns2Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(38, Male, MainApplicant),
              new Tuple<int, string, string>(38, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(16, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(8, Male, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(2, application);
        }

        [Test(Description = "Family with 2 children of the same sex 21yrs and over returns 3 bedrooms")]
        public void FamilyWith2ChildrenOfTheSameSex21yrsAndOverReturns3Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(55, Male, MainApplicant),
              new Tuple<int, string, string>(55, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(25, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(21, Male, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Family with 2 children of opposite sex under 10yrs returns 2 bedrooms")]
        public void FamilyWith2ChildrenOfOppositeSexUnder10yrsReturns2Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(25, Male, MainApplicant),
              new Tuple<int, string, string>(25, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(5, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(3, Female, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(2, application);
        }

        [Test(Description = "Family with 2 children of opposite sex with one aged 10yrs or over returns 3 bedrooms")]
        public void FamilyWith2ChildrenOfOppositeSexWithOneAged10yrsOrOverReturns3Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(40, Male, MainApplicant),
              new Tuple<int, string, string>(40, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(12, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(8, Female, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Family with 3 children of the same sex under 21yrs returns 3 bedrooms")]
        public void FamilyWith3ChildrenOfTheSameSexUnder21yrsReturns3Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(45, Male, MainApplicant),
              new Tuple<int, string, string>(45, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(16, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(12, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(5, Male, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Family with 3 children of the same sex with two children over 21yrs returns 4 bedrooms")]
        public void FamilyWith3ChildrenOfTheSameSexWithTwoChildrenOver21yrsReturns4Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(55, Male, MainApplicant),
              new Tuple<int, string, string>(55, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(28, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(25, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(16, Male, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(4, application);
        }

        [Test(Description = "Family with 3 children of opposite sex under 10yrs returns 3 bedrooms")]
        public void FamilyWith3ChildrenOfOppositeSexUnder10yrsReturns3Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(35, Male, MainApplicant),
              new Tuple<int, string, string>(35, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(3, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(8, Female, MainApplicantIsMyParent),
              new Tuple<int, string, string>(9, Male, MainApplicantIsMyParent),
            });

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Family with 3 children of opposite sex with 1 girl and 1 boy aged 10yrs or over returns 3 bedrooms")]
        public void FamilyWith3ChildrenOfOppositeSexWith1GirlAnd1BoyAged10yrsOrOverReturns3Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(35, Male, MainApplicant),
              new Tuple<int, string, string>(35, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(15, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(18, Female, MainApplicantIsMyParent),
              new Tuple<int, string, string>(5, Male, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Family with 3 children of opposite sex with 1 girl under 10yrs, 1 boy over 10yrs and 1 girl 21yrs or over returns 4 bedrooms")]
        public void FamilyWith3ChildrenOfOppositeSexWith1GirlUnder10yrs1BoyOver10yrsAnd1Girl21yrsOrOverReturns4Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(50, Male, MainApplicant),
              new Tuple<int, string, string>(50, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(21, Female, MainApplicantIsMyParent),
              new Tuple<int, string, string>(12, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(8, Female, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(4, application);
        }

        [Test(Description = "Family with 4 children of the same sex under 21yrs returns 3 bedrooms")]
        public void FamilyWith4ChildrenOfTheSameSexUnder21yrsReturns3Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(40, Male, MainApplicant),
              new Tuple<int, string, string>(40, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(5, Female, MainApplicantIsMyParent),
              new Tuple<int, string, string>(18, Female, MainApplicantIsMyParent),
              new Tuple<int, string, string>(12, Female, MainApplicantIsMyParent),
              new Tuple<int, string, string>(8, Female, MainApplicantIsMyParent),
            });

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Family with 4 children of the same sex with one child over 21yrs returns 4 bedrooms")]
        public void FamilyWith4ChildrenOfTheSameSexWithOneChildOver21yrsReturns4Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(45, Male, MainApplicant),
              new Tuple<int, string, string>(45, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(25, Female, MainApplicantIsMyParent),
              new Tuple<int, string, string>(16, Female, MainApplicantIsMyParent),
              new Tuple<int, string, string>(18, Female, MainApplicantIsMyParent),
              new Tuple<int, string, string>(5, Female, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(4, application);
        }

        [Test(Description = "Family with 4 children of the same sex with three child over 21yrs returns 5 bedrooms")]
        public void FamilyWith4ChildrenOfTheSameSexWithThreeChildOver21yrsReturns5Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(55, Male, MainApplicant),
              new Tuple<int, string, string>(55, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(28, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(25, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(21, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(16, Male, MainApplicantIsMyParent)
            });

            // assert
            AssertBedrooms(5, application);
        }

        [Test(Description = "Family with 4 children of opposite sex with all children under 21yrs returns 3 bedrooms")]
        public void FamilyWith4ChildrenOfOppositeSexWithAllChildrednUnder21yrsReturns3Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(55, Male, MainApplicant),
              new Tuple<int, string, string>(55, Female, MainApplicantIsPartner),
              new Tuple<int, string, string>(7, Female, MainApplicantIsMyParent),
              new Tuple<int, string, string>(7, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(14, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(15, Female, MainApplicantIsMyParent),
            });

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Single parent under 21 with 4 children of opposite sex with all children under 10yrs returns 3 bedrooms")]
        public void SingleParentUnder21With4ChildrenOfOppositeSexWithAllChildrednUnder21yrsReturns3Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(20, Female, MainApplicant),
              new Tuple<int, string, string>(3, Female, MainApplicantIsMyParent),
              new Tuple<int, string, string>(3, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(4, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(4, Female, MainApplicantIsMyParent),
            });

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Single parent under 21 with 3 children of opposite sex with all children under 10yrs returns 3 bedrooms")]
        public void SingleParentUnder21With3ChildrenOfOppositeSexWithAllChildrednUnder21yrsReturns3Bedrooms()
        {
            // arrange
            Application application = CreateApplication(new List<Tuple<int, string, string>>()
            {
              new Tuple<int, string, string>(20, Female, MainApplicant),
              new Tuple<int, string, string>(3, Female, MainApplicantIsMyParent),
              new Tuple<int, string, string>(3, Male, MainApplicantIsMyParent),
              new Tuple<int, string, string>(4, Female, MainApplicantIsMyParent),
            });

            // assert
            AssertBedrooms(3, application);
        }
        private static void AssertBedrooms(int expected, Application application)
        {
            // act
            var household = new List<Applicant> { application.MainApplicant }.Concat(application.OtherMembers);
            var actual = new BedroomCalculatorService().Calculate(household);

            // assert
            Assert.AreEqual(expected, actual);
        }

        private static Application CreateApplication(List<Tuple<int, string, string>> people)
        {
            var application = new Application();

            var mainApplicant = people.First(x => String.IsNullOrWhiteSpace(x.Item3));
            application.MainApplicant = CreateApplicant(mainApplicant.Item1, mainApplicant.Item2, "");

            var otherApplicants = people.Where(x => !String.IsNullOrWhiteSpace(x.Item3)).ToList();
            application.OtherMembers = otherApplicants.Select(x => CreateApplicant(x.Item1, x.Item2, x.Item3));

            return application;

            Applicant CreateApplicant(int age, string gender, string relationToMainApplicant)
            {
                return new Applicant
                {
                    Person = new Person
                    {
                        DateOfBirth = CalculateDob(age),
                        Gender = gender,
                        RelationshipType = relationToMainApplicant
                    }
                };
            }

            DateTime CalculateDob(int age)
            {
                return DateTime.UtcNow.AddYears(-age);
            }
        }
    }
}
