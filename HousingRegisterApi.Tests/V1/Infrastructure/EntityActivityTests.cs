using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Infrastructure;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class EntityActivityTests
    {
        [Test]
        public void AddingASimpleActivityDoesNotSetAnyOldData()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.EffectiveDateChanged);

            // Assert
            Assert.IsNull(entityActivity.OldData);
            Assert.IsNotNull(entityActivity.NewData);
        }

        [Test]
        public void AddingASimpleActivitySetsTheCorrectNewDataPayload()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.EffectiveDateChanged);

            // Assert
            AssertData(entityActivity.NewData, "{'type' : 8}");
        }

        [Test]
        public void AddingASimplePropertyChangeActivitySetsTheCorrectOldDataValue()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseActivated,
                "SimplePropertyType", 5, 10);

            // Assert
            AssertData(entityActivity.OldData, "{SimplePropertyType : 5}");
        }

        [Test]
        public void AddingASimplePropertyChangeActivitySetsTheCorrectNewDataValue()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseActivated,
                "SimplePropertyType", 5, 10);

            // Assert
            AssertData(entityActivity.NewData, "{'type': 4, 'payload' : {'SimplePropertyType' : 10}}");
        }

        [Test]
        public void AddingAComplexPropertyChangeActivitySetsTheCorrectOldDataValue()
        {
            // Arrange
            var origApplication = new Application
            {
                Reference = "12354",
                Assessment = new Assessment()
                {
                    BedroomNeed = 5,
                    InformationReceivedDate = DateTime.Now,
                    GenerateBiddingNumber = true,
                }
            };

            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseViewed,
                "Assessment.BedroomNeed", origApplication.Assessment.BedroomNeed, 10);

            // Assert
            AssertData(entityActivity.OldData, "{'Assessment.BedroomNeed' : 5 }");
        }

        [Test]
        public void AddingAComplexPropertyChangeActivitySetsTheCorrectNewDataValue()
        {
            // Arrange
            var origApplication = new Application
            {
                Reference = "12354",
                Assessment = new Assessment()
                {
                    BedroomNeed = 5,
                    InformationReceivedDate = DateTime.Now,
                    GenerateBiddingNumber = true,
                }
            };

            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseViewed,
                "Assessment.BedroomNeed", origApplication.Assessment.BedroomNeed, 10);

            // Assert
            AssertData(entityActivity.NewData, "{ 'type' : 1, 'payload' : {'Assessment.BedroomNeed' : 10 }}");
        }

        [Test]
        public void AddingASimplePropertyChangeActivityWithANullOldDataSetsTheCorrectOldDataValue()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseActivated,
                "SimplePropertyType", null, 10);

            // Assert
            AssertData(entityActivity.OldData, "{'SimplePropertyType' : null}");
        }

        [Test]
        public void AddingASimplePropertyChangeActivityWithANullNewDataSetsTheCorrectNewDataValue()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseActivated,
                "SimplePropertyType", 5, null);

            // Assert
            AssertData(entityActivity.NewData, "{'type' : 4, 'payload' : {'SimplePropertyType' : null }}");
        }

        [Test]
        public void AddingACollectionOfActivitiesSetsTheCorrectOldDataPayload()
        {
            // Act
            var collection = new EntityActivityCollection<ApplicationActivityType>();
            collection.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseActivated, "SimplePropertyType", 5, null));
            collection.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseViewed, "SimplePropertyType2", 25, null));

            // Assert
            AssertArrayData(collection.OldData, "[{'SimplePropertyType' : 5}, {'SimplePropertyType2' : 25}]");
        }

        [Test]
        public void AddingACollectionOfActivitiesSetsTheCorrectNewDataPayload()
        {
            // Act
            var collection = new EntityActivityCollection<ApplicationActivityType>();
            collection.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseActivated, "SimplePropertyType", null, 40));
            collection.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.StatusChanged, "SimplePropertyType2", null, 45));

            // Assert
            AssertArrayData(collection.NewData, "[{ 'type' : 4, 'payload' : {'SimplePropertyType' : 40}}, { 'type' : 2, 'payload' : {'SimplePropertyType2' : 45}}]");
        }

        [Test]
        public void AddingACollectionOfActivitiesWithSomeSimpleActivitiesSetsTheCorrectOldDataPayload()
        {
            // Act
            var collection = new EntityActivityCollection<ApplicationActivityType>();
            collection.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseActivated));
            collection.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.StatusChanged, "SimplePropertyType", 5, null));

            // Assert
            AssertArrayData(collection.OldData, "[{'SimplePropertyType' : 5}]");
        }

        [Test]
        public void AddingACollectionOfActivitiesWithSomeSimpleActivitiesSetsTheCorrectNewDataPayload()
        {
            // Act
            var collection = new EntityActivityCollection<ApplicationActivityType>();
            collection.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseActivated));
            collection.Add(new EntityActivity<ApplicationActivityType>(ApplicationActivityType.StatusChanged, "SimplePropertyType", null, 45));

            // Assert
            AssertArrayData(collection.NewData, "[{ 'type' : 4}, { 'type' : 2, 'payload' : {'SimplePropertyType' : 45}}]");
        }

        private static void AssertData(object input, string compareTo)
        {
            JObject jInput = JObject.FromObject(input);
            JObject jCompare = JObject.Parse(compareTo);

            Assert.IsTrue(JToken.DeepEquals(jInput, jCompare));
        }

        private static void AssertArrayData(object input, string compareTo)
        {
            JArray jInput = JArray.FromObject(input);
            JArray jCompare = JArray.Parse(compareTo);

            Assert.IsTrue(JToken.DeepEquals(jInput, jCompare));
        }
    }
}
