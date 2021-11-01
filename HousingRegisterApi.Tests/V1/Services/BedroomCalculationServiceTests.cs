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
            var actual = BedroomCalculationService.Calculate(application);

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
            var actual = BedroomCalculationService.Calculate(application);

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
            var actual = BedroomCalculationService.Calculate(application);

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
            var actual = BedroomCalculationService.Calculate(application);

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
            var actual = BedroomCalculationService.Calculate(application);

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
            var actual = BedroomCalculationService.Calculate(application);

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

            AssertBedrooms(3, application);
        }

        [Test(Description = "Single person under 35yrs returns 0 bedrooms")]
        public void SinglePersonUnder35yrsReturns0Bedrooms()
        {
            // arrange
            bool partnerSharing = false;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(25, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(0, application);
        }

        [Test(Description = "Couple returns 1 bedrooms")]
        public void CoupleReturns1Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(25, Male),
              new Tuple<int, string>(25, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(1, application);
        }

        [Test(Description = "Single person 35yrs Plus returns 1 bedrooms")]
        public void SinglePerson35yrsPlusReturns1Bedrooms()
        {
            // arrange
            bool partnerSharing = false;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(35, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(1, application);
        }

        [Test(Description = "Single person returns 1 bedrooms")]
        public void SinglePersonReturns1Bedrooms()
        {
            // arrange
            bool partnerSharing = false;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(25, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(1, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Single person (social tenant only) returns 1 bedrooms")]
        public void SinglePersonSocialTenantOnlyReturns1Bedrooms()
        {
            // arrange
            bool partnerSharing = false;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(1, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Single person not a social tenant returns 1 bedrooms")]
        public void SinglePersonNotASocialTenantReturns1Bedrooms()
        {
            // arrange
            bool partnerSharing = false;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(1, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Couple (social tenant only) returns 1 bedrooms")]
        public void CoupleSocialTenantOnlyReturns1Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(1, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Couple not a social tenant returns 1 bedrooms")]
        public void CoupleNotASocialTenantReturns1Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(1, application);
        }

        [Test(Description = "Family with 1 child returns 2 bedrooms")]
        public void FamilyWith1ChildReturns2Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(25, Male),
              new Tuple<int, string>(25, Female),
              new Tuple<int, string>(5, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(2, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 1 child (social tenant only) returns 2 bedrooms")]
        public void FamilyWith1ChildSocialTenantOnlyReturns2Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(2, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 1 child and not a social tenant returns 2 bedrooms")]
        public void FamilyWith1ChildAndNotASocialTenantReturns2Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(2, application);
        }

        [Test(Description = "Family with 2 children of the same sex under 21yrs returns 2 bedrooms")]
        public void FamilyWith2ChildrenOfTheSameSexUnder21yrsReturns2Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(38, Male),
              new Tuple<int, string>(38, Female),
              new Tuple<int, string>(16, Male),
              new Tuple<int, string>(8, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(2, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 2 children of the same sex under 21yrs (social tenant only) returns 2 bedrooms")]
        public void FamilyWith2ChildrenOfTheSameSexUnder21yrsSocialTenantOnlyReturns2Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(2, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 2 children of the same sex under 21yrs and not a social tenant returns 2 bedrooms")]
        public void FamilyWith2ChildrenOfTheSameSexUnder21yrsAndNotASocialTenantReturns2Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(2, application);
        }

        [Test(Description = "Family with 2 children of the same sex 21yrs and over returns 3 bedrooms")]
        public void FamilyWith2ChildrenOfTheSameSex21yrsAndOverReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(55, Male),
              new Tuple<int, string>(55, Female),
              new Tuple<int, string>(25, Male),
              new Tuple<int, string>(21, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 2 children of the same sex 21yrs and over (social tenants only) returns 3 bedrooms")]
        public void FamilyWith2ChildrenOfTheSameSex21yrsAndOverSocialTenantsOnlyReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 2 children of the same sex 21yrs and over and not a social tenant returns 3 bedrooms")]
        public void FamilyWith2ChildrenOfTheSameSex21yrsAndOverAndNotASocialTenantReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Family with 2 children of opposite sex under 10yrs returns 2 bedrooms")]
        public void FamilyWith2ChildrenOfOppositeSexUnder10yrsReturns2Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(25, Male),
              new Tuple<int, string>(25, Female),
              new Tuple<int, string>(5, Male),
              new Tuple<int, string>(3, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(2, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 2 children of opposite sex under 10yrs (Social tenants only) returns 2 bedrooms")]
        public void FamilyWith2ChildrenOfOppositeSexUnder10yrsSocialTenantsOnlyReturns2Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(2, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 2 children of opposite sex under 10yrs and not a social tenant returns 2 bedrooms")]
        public void FamilyWith2ChildrenOfOppositeSexUnder10yrsAndNotASocialTenantReturns2Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(2, application);
        }

        [Test(Description = "Family with 2 children of opposite sex with one aged 10yrs or over returns 3 bedrooms")]
        public void FamilyWith2ChildrenOfOppositeSexWithOneAged10yrsOrOverReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(40, Male),
              new Tuple<int, string>(40, Female),
              new Tuple<int, string>(12, Male),
              new Tuple<int, string>(8, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 2 children of opposite sex with one aged 10yrs or over (Social tenants only) returns 3 bedrooms")]
        public void FamilyWith2ChildrenOfOppositeSexWithOneAged10yrsOrOverSocialTenantsOnlyReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 2 children of opposite sex with one aged 10yrs or over and not a social tenant returns 3 bedrooms")]
        public void FamilyWith2ChildrenOfOppositeSexWithOneAged10yrsOrOverAndNotASocialTenantReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Family with 3 children of the same sex under 21yrs returns 3 bedrooms")]
        public void FamilyWith3ChildrenOfTheSameSexUnder21yrsReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(45, Male),
              new Tuple<int, string>(45, Female),
              new Tuple<int, string>(16, Male),
              new Tuple<int, string>(12, Male),
              new Tuple<int, string>(5, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 3 children of the same sex under 21yrs (Social tenants only) returns 3 bedrooms")]
        public void FamilyWith3ChildrenOfTheSameSexUnder21yrsSocialTenantsOnlyReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 3 children of the same sex under 21yrs and not a social tenant returns 3 bedrooms")]
        public void FamilyWith3ChildrenOfTheSameSexUnder21yrsAndNotASocialTenantReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Family with 3 children of the same sex with two children over 21yrs returns 4 bedrooms")]
        public void FamilyWith3ChildrenOfTheSameSexWithTwoChildrenOver21yrsReturns4Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(55, Male),
              new Tuple<int, string>(55, Female),
              new Tuple<int, string>(28, Male),
              new Tuple<int, string>(25, Male),
              new Tuple<int, string>(16, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(4, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 3 children of the same sex with two children over 21yrs (social tenants only) returns 4 bedrooms")]
        public void FamilyWith3ChildrenOfTheSameSexWithTwoChildrenOver21yrsSocialTenantsOnlyReturns4Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(4, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 3 children of the same sex with two children over 21yrs and not a social tenant returns 4 bedrooms")]
        public void FamilyWith3ChildrenOfTheSameSexWithTwoChildrenOver21yrsAndNotASocialTenantReturns4Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(4, application);
        }

        [Test(Description = "Family with 3 children of opposite sex under 10yrs returns 3 bedrooms")]
        public void FamilyWith3ChildrenOfOppositeSexUnder10yrsReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(35, Male),
              new Tuple<int, string>(35, Female),
              new Tuple<int, string>(9, Male),
              new Tuple<int, string>(8, Female),
              new Tuple<int, string>(3, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 3 children of opposite sex under 10yrs (social tenants only) returns 3 bedrooms")]
        public void FamilyWith3ChildrenOfOppositeSexUnder10yrsSocialTenantsOnlyReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 3 children of opposite sex under 10yrs and is not a social tenant returns 3 bedrooms")]
        public void FamilyWith3ChildrenOfOppositeSexUnder10yrsAndIsNotASocialTenantReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Family with 3 children of opposite sex with 1 girl and 1 boy aged 10yrs or over returns 3 bedrooms")]
        public void FamilyWith3ChildrenOfOppositeSexWith1GirlAnd1BoyAged10yrsOrOverReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(35, Male),
              new Tuple<int, string>(35, Female),
              new Tuple<int, string>(15, Male),
              new Tuple<int, string>(18, Female),
              new Tuple<int, string>(5, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 3 children of opposite sex with 1 girl and 1 boy aged 10yrs or over (social tenant only) returns 3 bedrooms")]
        public void FamilyWith3ChildrenOfOppositeSexWith1GirlAnd1BoyAged10yrsOrOverSocialTenantOnlyReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 3 children of opposite sex with 1 girl and 1 boy aged 10yrs or over and is not a social tenant returns 3 bedrooms")]
        public void FamilyWith3ChildrenOfOppositeSexWith1GirlAnd1BoyAged10yrsOrOverAndIsNotASocialTenantReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Test(Description = "Family with 3 children of opposite sex with 1 girl under 10yrs, 1 boy over 10yrs and 1 girl 21yrs or over returns 4 bedrooms")]
        public void FamilyWith3ChildrenOfOppositeSexWith1GirlUnder10yrs1BoyOver10yrsAnd1Girl21yrsOrOverReturns4Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(50, Male),
              new Tuple<int, string>(50, Female),
              new Tuple<int, string>(21, Female),
              new Tuple<int, string>(12, Male),
              new Tuple<int, string>(8, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(4, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 3 children of opposite sex with 1 girl under 10yrs, 1 boy over 10yrs and 1 girl 21yrs or over (social tenant only) returns 4 bedrooms")]
        public void FamilyWith3ChildrenOfOppositeSexWith1GirlUnder10yrs1BoyOver10yrsAnd1Girl21yrsOrOverSocialTenantOnlyReturns4Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(4, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 3 children of opposite sex with 1 girl under 10yrs  1 boy over 10yrs and 1 girl 21yrs or over and not a social tenant returns 4 bedrooms")]
        public void FamilyWith3ChildrenOfOppositeSexWith1GirlUnder10yrs1BoyOver10yrsAnd1Girl21yrsOrOverAndNotASocialTenantReturns4Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(4, application);
        }

        [Test(Description = "Family with 4 children of the same sex under 21yrs returns 3 bedrooms")]
        public void FamilyWith4ChildrenOfTheSameSexUnder21yrsReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(40, Male),
              new Tuple<int, string>(40, Female),
              new Tuple<int, string>(18, Female),
              new Tuple<int, string>(12, Female),
              new Tuple<int, string>(8, Female),
              new Tuple<int, string>(5, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 4 children of the same sex under 21yrs (social tenant only) returns 3 bedrooms")]
        public void FamilyWith4ChildrenOfTheSameSexUnder21yrsSocialTenantOnlyReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 4 children of the same sex under 21yrs and is not a social tenant returns 3 bedrooms")]
        public void FamilyWith4ChildrenOfTheSameSexUnder21yrsAndIsNotASocialTenantReturns3Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(3, application);
        }       

        [Test(Description = "Family with 4 children of the same sex with one child over 21yrs returns 4 bedrooms")]
        public void FamilyWith4ChildrenOfTheSameSexWithOneChildOver21yrsReturns4Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(45, Male),
              new Tuple<int, string>(45, Female),
              new Tuple<int, string>(25, Female),
              new Tuple<int, string>(18, Female),
              new Tuple<int, string>(16, Female),
              new Tuple<int, string>(5, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(4, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 4 children of the same sex with one child over 21yrs (social tenants only) returns 4 bedrooms")]
        public void FamilyWith4ChildrenOfTheSameSexWithOneChildOver21yrsSocialTenantsOnlyReturns4Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(4, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 4 children of the same sex with one child over 21yrs and is not a social tenant returns 4 bedrooms")]
        public void FamilyWith4ChildrenOfTheSameSexWithOneChildOver21yrsAndIsNotASocialTenantReturns4Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(4, application);
        }

        [Test(Description = "Family with 4 children of the same sex with three child over 21yrs returns 5 bedrooms")]
        public void FamilyWith4ChildrenOfTheSameSexWithThreeChildOver21yrsReturns5Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(55, Male),
              new Tuple<int, string>(55, Female),
              new Tuple<int, string>(28, Male),
              new Tuple<int, string>(25, Male),
              new Tuple<int, string>(21, Male),
              new Tuple<int, string>(16, Male)
            }, partnerSharing);

            // assert
            AssertBedrooms(5, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 4 children of the same sex with three child over 21yrs (social tenants only) returns 5 bedrooms")]
        public void FamilyWith4ChildrenOfTheSameSexWithThreeChildOver21yrsSocialTenantsOnlyReturns5Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(5, application);
        }

        [Ignore("What is a social tenant")]
        [Test(Description = "Family with 4 children of the same sex with three child over 21yrs and is not a social tenant) returns 5 bedrooms")]
        public void FamilyWith4ChildrenOfTheSameSexWithThreeChildOver21yrsAndIsNotASocialTenantReturns5Bedrooms()
        {
            // arrange
            bool partnerSharing = true;
            Application application = CreateApplication(new List<Tuple<int, string>>()
            {
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female),
              new Tuple<int, string>(0, Male),
              new Tuple<int, string>(0, Female)
            }, partnerSharing);

            // assert
            AssertBedrooms(5, application);
        }


        private static void AssertBedrooms(int expected, Application application)
        {
            // act
            var actual = BedroomCalculationService.Calculate(application);

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
