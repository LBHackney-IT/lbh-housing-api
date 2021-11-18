using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class EntityActivity<TActivityType>
        where TActivityType : Enum
    {
        /// <summary>
        /// Type of activity being logged
        /// </summary>
        public TActivityType ActivityType { get; private set; }

        /// <summary>
        /// Stores the previous state, if needed.
        /// </summary>
        public string OldData { get; private set; }

        /// <summary>
        /// Stores the new state
        /// </summary>
        public string NewData { get; private set; }

        /// <summary>
        /// Signifies if activity requires to hold state information
        /// </summary>
        public bool StoreState { get; private set; }


        public EntityActivity(TActivityType activityType)
        {
            ActivityType = activityType;
            StoreState = false;
            SetOldData(null, null);
            SetNewData(null, activityType, null);
        }

        public EntityActivity(TActivityType activityType, string propertyName,
            object originalPropertyValue, object newPropertyValue)
        {
            ActivityType = activityType;
            StoreState = true;
            SetOldData(propertyName, originalPropertyValue);
            SetNewData(propertyName, activityType, newPropertyValue);
        }

        /// <summary>
        /// Compares the old and new data for changes
        /// </summary>
        /// <returns></returns>
        public bool HasChanges()
        {
            if (StoreState)
            {
                JToken obj1 = JObject.Parse(OldData);
                JToken obj2 = JObject.Parse(NewData).SelectToken("payload");
                return !JToken.DeepEquals(obj1, obj2);
            }
            else
            {
                return true;
            }
        }

        private void SetOldData(string propertyName, object originalPropertyValue)
        {
            // set old data
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                JObject jObjOld = new JObject();
                jObjOld.Add(propertyName, originalPropertyValue == null ? null : JToken.FromObject(originalPropertyValue));
                OldData = jObjOld.ToString(Formatting.None);
            }
            else
            {
                OldData = null;
            }
        }

        private void SetNewData(string propertyName, TActivityType activityType, object newPropertyValue)
        {
            // set activity type
            JObject jObjNew = new JObject();
            jObjNew.Add(new JProperty("type", activityType));

            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                // set payload
                JObject jObjNewValue = new JObject();
                jObjNewValue.Add(propertyName, newPropertyValue == null ? null : JToken.FromObject(newPropertyValue));
                jObjNew.Add(new JProperty("payload", jObjNewValue));
            }

            NewData = jObjNew.ToString(Formatting.None);
        }
    }
}
