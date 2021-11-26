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

        /// <summary>
        /// Creates a new EntityActivity object to record an event     
        /// </summary>
        /// <param name="activityType"></param>
        public EntityActivity(TActivityType activityType)
        {
            ActivityType = activityType;
            StoreState = false;
            OldData = new Dictionary<string, object>();
            NewData = new Dictionary<string, object>
            {
                // set activity type
                { "_activityType", activityType }
            };
        }

        /// <summary>
        /// Creates a new EntityActivity object to record an event with data
        /// ie. MainApplicant.Person.Title
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="data"></param>
        public EntityActivity(TActivityType activityType, string data)
        {
            ActivityType = activityType;
            StoreState = false;
            OldData = new Dictionary<string, object>();
            NewData = new Dictionary<string, object>
            {
                // set activity type
                { "_activityType", activityType }
            };

            AddNewState("activityData", data);
        }

        /// <summary>
        /// Creates a new EntityActivity object to record status changes for an entity
        /// Make sure the property name is in the dot notation format, so that the ui can pick up the field from the entity object
        /// ie. MainApplicant.Person.Title
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="propertyName"></param>
        /// <param name="originalPropertyValue"></param>
        /// <param name="newPropertyValue"></param>
        public EntityActivity(TActivityType activityType, string propertyName,
            object originalPropertyValue, object newPropertyValue)
        {
            ActivityType = activityType;
            StoreState = true;
            OldData = new Dictionary<string, object>();
            NewData = new Dictionary<string, object>
            {
                // set activity type
                { "_activityType", activityType }
            };

            AddOldState(propertyName, originalPropertyValue);
            AddNewState(propertyName, newPropertyValue);
        }

        /// <summary>
        /// Compares the old and new data for changes
        /// </summary>
        /// <returns></returns>
        public bool HasChanges()
        {
            if (StoreState)
            {
                var newData = new Dictionary<string, object>(NewData.Where(x => x.Key != "_activityType"));

                // compare the old and new values
                if (OldData.Count != newData.Count)
                {
                    return true;
                }

                bool equal = true;

                foreach (var oldValue in OldData)
                {
                    if (newData.TryGetValue(oldValue.Key, out object newValue))
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

        /// <summary>
        /// Adds a new state change.
        /// Make sure the property name is in the dot notation format, so that the ui can pick up the field from the entity object
        /// ie. MainApplicant.Person.Title
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="originalPropertyValue"></param>
        /// <param name="newPropertyValue"></param>
        public void AddChange(string propertyName, object originalPropertyValue, object newPropertyValue)
        {
            AddOldState(propertyName, originalPropertyValue);
            AddNewState(propertyName, newPropertyValue);
        }

        private void AddOldState(string propertyName, object originalPropertyValue)
        {
            // set old data
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                OldData.Add(CamelCase(propertyName), Clone(originalPropertyValue));
            }
        }

        private void AddNewState(string propertyName, object newPropertyValue)
        {
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                // set new state              
                NewData.Add(CamelCase(propertyName), Clone(newPropertyValue));
            }
        }

        private static object Clone(object source)
        {
            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(source));
        }

        private static string CamelCase(string propertyName)
        {
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                List<string> propertyNames = propertyName.Split(".")
                    .Select(x => char.ToLowerInvariant(x[0]) + x[1..])
                    .ToList();

                return string.Join(".", propertyNames);
            }
            return propertyName;
        }
    }
}
