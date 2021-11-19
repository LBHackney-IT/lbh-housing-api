using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public Dictionary<string, object> OldData { get; private set; }

        /// <summary>
        /// Stores the new state
        /// </summary>
        public Dictionary<string, object> NewData { get; private set; }

        /// <summary>
        /// Signifies if activity requires to hold state information
        /// </summary>
        public bool StoreState { get; private set; }

        public EntityActivity(TActivityType activityType)
        {
            ActivityType = activityType;
            StoreState = false;
            OldData = new Dictionary<string, object>();
            NewData = new Dictionary<string, object>();
            SetOldData(null, null);
            SetNewData(null, activityType, null);
        }

        public EntityActivity(TActivityType activityType, string propertyName,
            object originalPropertyValue, object newPropertyValue)
        {
            ActivityType = activityType;
            StoreState = true;
            OldData = new Dictionary<string, object>();
            NewData = new Dictionary<string, object>();
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
                var newData = new Dictionary<string, object>(NewData.Where(x => x.Key != "_ActivityType"));

                // compare the old and new values
                if (OldData.Count != newData.Count)
                {
                    return true;
                }

                bool equal = true;

                foreach (var oldValue in OldData)
                {
                    object newValue;
                    if (newData.TryGetValue(oldValue.Key, out newValue))
                    {
                        if (newValue?.ToString() != oldValue.Value?.ToString())
                        {
                            equal = false;
                            break;
                        }
                    }
                    else
                    {
                        equal = false;
                        break;
                    }
                }

                return !equal;
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
                OldData.Add(propertyName, Clone(originalPropertyValue));
            }
        }

        private void SetNewData(string propertyName, TActivityType activityType, object newPropertyValue)
        {
            // set activity type
            NewData.Add("_ActivityType", activityType);

            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                // set new state              
                NewData.Add(propertyName, Clone(newPropertyValue));
            }
        }

        private static object Clone(object source)
        {
            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(source));
        }
    }
}
