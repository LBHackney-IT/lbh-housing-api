using HousingRegisterApi.V1.Domain;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.Tests.V1.Domain
{
    [TestFixture]
    public class ApplicationTests
    {
        private const string Male = "male";
        private const string Female = "female";

        [Test(Description = "Should award 1 bedroom for different genders under the age of 10")]
        public void ApplicationShouldAward1BedroomForDifferentGendersUnderTheAgeOf10()
        {
            // arrange
            bool partnerSharing = false;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
                new Tuple<int, string>(5, Male),
                new Tuple<int, string>(5, Female)
            }, partnerSharing);

            // act
            var expected = 1;
            var actual = application.CalculateBedrooms();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Children of different genders where one is over 10 should be awarded 2 bedrooms")]
        public void ApplicationWithChildrenOfDifferentGendersWhereOneIsOver10ShouldBeAwarded2Bedrooms()
        {
            // arrange
            bool partnerSharing = false;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
                new Tuple<int, string>(5, Male),
                new Tuple<int, string>(10, Female)
            }, partnerSharing);

            // act
            var expected = 2;
            var actual = application.CalculateBedrooms();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Children of ages between 5 to 15 of different genders should be awarded 2 bedrooms")]
        public void ApplicationWithChildrenOfAgesBetween5To15OfDifferentGendersShouldBeAwarded2Bedrooms()
        {
            // arrange
            bool partnerSharing = false;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
                new Tuple<int, string>(5, Male),
                new Tuple<int, string>(15, Male),
                new Tuple<int, string>(10, Female)
            }, partnerSharing);

            // act
            var expected = 2;
            var actual = application.CalculateBedrooms();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "2 boys aged between 5 and 15 and 2 girls aged 10 and 12 should be awarded 2 bedrooms")]
        public void ApplicationWith2BoysAgedBetween5And15And2GirlsAged10And12ShouldBeAwarded2Bedrooms()
        {
            // arrange
            bool partnerSharing = false;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
                new Tuple<int, string>(5, Male),
                new Tuple<int, string>(15, Male),
                new Tuple<int, string>(10, Female),
                new Tuple<int, string>(12, Female)
            }, partnerSharing);

            // act
            var expected = 2;
            var actual = application.CalculateBedrooms();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "should calculate correct amount of bedrooms for couple without children")]
        public void ApplicationShouldCalculateCorrectAmountOfBedroomsForCoupleWithoutChildren()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
                new Tuple<int, string>(30, Male),
                new Tuple<int, string>(25, Female)
            }, partnerSharing);

            // act
            var expected = 1;
            var actual = application.CalculateBedrooms();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "should calculate 2 bedrooms for couple with children under the age of 10(different gender)")]
        public void ApplicationShouldCalculate2BedroomsForCoupleWithChildrenUnderTheAgeOf10WithDifferentGender()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
                new Tuple<int, string>(30, Male),
                new Tuple<int, string>(25, Female),
                new Tuple<int, string>(5, Male),
                new Tuple<int, string>(7, Female)
            }, partnerSharing);

            // act
            var expected = 2;
            var actual = application.CalculateBedrooms();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "should calculate 3 bedrooms for couple with children over the age of 10(different gender)")]
        public void ApplicationShouldCalculate3BedroomsForCoupleWithChildrenOverTheAgeOf10WithDifferentGender()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
                new Tuple<int, string>(30, Male),
                new Tuple<int, string>(25, Female),
                new Tuple<int, string>(11, Male),
                new Tuple<int, string>(12, Female)
            }, partnerSharing);

            // act
            var expected = 3;
            var actual = application.CalculateBedrooms();

            // assert
            Assert.AreEqual(expected, actual);
        }

        private static Application CreateApplication(List<Tuple<int, string>> people, bool mainApplicantHasPartnerSharing)
        {
            var application = new Application();

            var mainApplicant = people.First();
            application.MainApplicant = CreateApplicant(mainApplicant.Item1, mainApplicant.Item2, mainApplicantHasPartnerSharing);

            var otherApplicants = people.Skip(1).ToList();
            application.OtherMembers = otherApplicants.Select(x => CreateApplicant(x.Item1, x.Item2, false));

            return application;

            Applicant CreateApplicant(int age, string gender, bool hasPartnerSharing)
            {
                return new Applicant
                {
                    Person = new Person
                    {
                        DateOfBirth = CalculateDob(age),
                        Gender = gender,
                        RelationshipType = hasPartnerSharing ? "partner" : string.Empty
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
