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
    }
}
