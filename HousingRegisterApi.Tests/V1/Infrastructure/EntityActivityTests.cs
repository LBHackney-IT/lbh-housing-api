using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Infrastructure;
using NUnit.Framework;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class EntityActivityTests
    {
        [Test]
        public void AddingASimpleActivityDoesNotSetAnyOldData()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.EffectiveDateChangedByUser);

            // Assert
            Assert.IsTrue(entityActivity.OldData.Count == 0);
            Assert.IsTrue(entityActivity.NewData.Count == 1);
            Assert.IsTrue(entityActivity.HasChanges());
        }

        [Test]
        public void AddingASimpleActivitySetsTheCorrectNewDataPayload()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.EffectiveDateChangedByUser);

            // Assert
            Assert.IsNull(entityActivity.OldData);
            AssertIsEqual(entityActivity.NewData, "{\"_activityType\" : \"EffectiveDateChangedByUser\"}");
            Assert.IsTrue(entityActivity.HasChanges());
        }

        [Test]
        public void AddingASimplePropertyChangeActivitySetsTheCorrectOldDataValue()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.SensitivityChangedByUser,
                "SimplePropertyType", 5, 10);

            // Assert
            AssertIsEqual(entityActivity.OldData, "{\"simplePropertyType\" : 5}");
            Assert.IsTrue(entityActivity.HasChanges());
        }

        [Test]
        public void AddingASimplePropertyChangeActivitySetsTheCorrectNewDataValue()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.SensitivityChangedByUser,
                "SimplePropertyType", 5, 10);

            // Assert
            AssertIsEqual(entityActivity.NewData, "{\"_activityType\": \"SensitivityChangedByUser\", \"simplePropertyType\" : 10}");
            Assert.IsTrue(entityActivity.HasChanges());
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
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseViewedByUser,
                "Assessment.BedroomNeed", origApplication.Assessment.BedroomNeed, 10);

            // Assert
            AssertIsEqual(entityActivity.OldData, "{\"assessment.bedroomNeed\" : 5 }");
            Assert.IsTrue(entityActivity.HasChanges());
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
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseViewedByUser,
                "Assessment.BedroomNeed", origApplication.Assessment.BedroomNeed, 10);

            // Assert
            AssertIsEqual(entityActivity.NewData, "{ \"_activityType\" : \"CaseViewedByUser\", \"assessment.bedroomNeed\" : 10 }");
            Assert.IsTrue(entityActivity.HasChanges());
        }

        [Test]
        public void AddingASimplePropertyChangeActivityWithANullOldDataSetsTheCorrectOldDataValue()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.SensitivityChangedByUser,
                "SimplePropertyType", null, 10);

            // Assert
            AssertIsEqual(entityActivity.OldData, "{\"simplePropertyType\" : null}");
            Assert.IsTrue(entityActivity.HasChanges());
        }

        [Test]
        public void AddingASimplePropertyChangeActivityWithANullNewDataSetsTheCorrectNewDataValue()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.SensitivityChangedByUser,
                "SimplePropertyType", 5, null);

            // Assert
            AssertIsEqual(entityActivity.NewData, "{\"_activityType\" : \"SensitivityChangedByUser\", \"simplePropertyType\" : null }");
            Assert.IsTrue(entityActivity.HasChanges());
        }

        [Test]
        public void AnActivityWithTheSameOldAndNewDataHasNoChanges()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.SensitivityChangedByUser,
                "SimplePropertyType", 5, 5);

            // Assert
            Assert.IsFalse(entityActivity.HasChanges());
        }

        [Test]
        public void AnActivityWithDifferentOldAndNewDataHasChanges()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.SensitivityChangedByUser,
                "SimplePropertyType", 5, 10);

            // Assert
            Assert.IsTrue(entityActivity.HasChanges());
        }

        [Test]
        public void AddingAComplexPropertyWithSameOldAndNewDataHasChanges()
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
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.CaseViewedByUser,
                "Assessment.BedroomNeed", origApplication.Assessment.BedroomNeed, 5);

            entityActivity.AddChange("Assessment.GenerateBiddingNumber",
                origApplication.Assessment.GenerateBiddingNumber, true);

            // Assert
            AssertIsEqual(entityActivity.NewData, "{ \"_activityType\" : \"CaseViewedByUser\", \"assessment.bedroomNeed\" : 5, \"assessment.generateBiddingNumber\" : true}");
            Assert.IsFalse(entityActivity.HasChanges());
        }

        [Test]
        public void ChangingStateFromActiveToPendingWithReasonSetsTheCorrectNewDataValue()
        {
            // Act
            var entityActivity = new EntityActivity<ApplicationActivityType>(ApplicationActivityType.StatusChangedByUser,
                "Status", ApplicationStatus.ActiveUnderAppeal, ApplicationStatus.Pending);

            entityActivity.AddChange("Assessment.Reason", null, "Needs investigation");

            // Assert
            AssertIsEqual(entityActivity.OldData, "{\"status\" : \"ActiveUnderAppeal\", \"assessment.reason\" : null}");
            AssertIsEqual(entityActivity.NewData, "{\"_activityType\" : \"StatusChangedByUser\", \"status\" : \"Pending\", \"assessment.reason\" : \"Needs investigation\"}");
            Assert.IsTrue(entityActivity.HasChanges());
        }

        private void AssertIsEqual(object input, string compareTo)
        {
            var options = CreateJsonOptions();

            var inputAsString = JsonSerializer.Serialize(input, options);
            var compareToAsString = JsonSerializer.Serialize(JsonSerializer.Deserialize<object>(compareTo, options), options);

            Assert.IsTrue(inputAsString.Equals(compareToAsString));
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}
