using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using FluentAssertions;
using HousingRegisterApi.V1.Infrastructure;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class DynamoDBSearchHelperTests
    {
        private Mock<DynamoDBSearchHelper> _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new Mock<DynamoDBSearchHelper>();
        }

        [TestCase("Mr A.N.Other")]
        [TestCase("Mrs A n other")]
        [TestCase("Dr A. Other")]
        [TestCase("A Other")]
        [TestCase("Other")]
        public void SearchTermReturnsMainApplicatSurnameConditions(string searchTerm)
        {
            // Arrange
            var expectedResult = new List<ScanCondition>
            {
                { new ScanCondition("MainApplicant.Person.Surname", ScanOperator.Equal, searchTerm) }
            };

            // Act
            var conditions = _classUnderTest.Object.Execute(searchTerm);

            // Assert
            conditions.Should().BeEquivalentTo(expectedResult);
        }

        [TestCase("AB 12 34 56 C")]
        [TestCase("AB123456C")]
        public void SearchTermReturnsNationalInsuranceConditions(string searchTerm)
        {
            // Arrange
            var expectedResult = new List<ScanCondition>
            {
                { new ScanCondition("MainApplicant.Person.NationalInsuranceNumber", ScanOperator.Equal, searchTerm) }
            };

            // Act
            var conditions = _classUnderTest.Object.Execute(searchTerm);

            // Assert
            conditions.Should().BeEquivalentTo(expectedResult);
        }

        [TestCase("7ae4f7b54e")]
        [TestCase("43hhb43yub")]
        [TestCase("gb34iuiu34")]
        [TestCase("fb4u3ie4tt")]
        public void SearchTermReturnsReferenceConditions(string searchTerm)
        {
            // Arrange
            var expectedResult = new List<ScanCondition>
            {
                { new ScanCondition("Reference", ScanOperator.Equal, searchTerm) }
            };

            // Act
            var conditions = _classUnderTest.Object.Execute(searchTerm);

            // Assert
            conditions.Should().BeEquivalentTo(expectedResult);
        }

        //[TestCase(new Guid("21a9ff74-d78c-4925-88f4-ad3dc506998c").ToString())]
        //[TestCase("88bac302-ccde-4c53-a29f-5be6a4fbe824")]
        [Test]
        public void SearchTermReturnsIdConditions()
        {
            // Arrange

            var guid = Guid.NewGuid();
            var expectedResult = new List<ScanCondition>
            {
                { new ScanCondition("Id", ScanOperator.Equal, guid.ToString()) }
            };

            // Act
            var conditions = _classUnderTest.Object.Execute(guid.ToString());

            // Assert
            conditions.Should().BeEquivalentTo(expectedResult);
        }
    }
}
