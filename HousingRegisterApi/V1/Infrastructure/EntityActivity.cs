using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class EntityActivityCollection<TActivityType>
        where TActivityType : Enum
    {
        private List<EntityActivity<TActivityType>> _activities;

        /// <summary>
        /// Combines all original data, prior to updates, into a single object
        /// </summary>
        /// <returns></returns>
        public object OldData { get; private set; }

        /// <summary>
        /// Combines all new data changes into a single object
        /// </summary>
        /// <returns></returns>
        public object NewData { get; private set; }

        /// <summary>
        /// Initialises a new instance    
        /// </summary>
        public EntityActivityCollection()
        {
            _activities = new List<EntityActivity<TActivityType>>();
        }

        /// <summary>
        /// Add new activity type to the in-memory collection
        /// </summary>
        /// <param name="activity"></param>
        public void Add(EntityActivity<TActivityType> activity)
        {
            _activities.Add(activity);

            JObject jOldDataCombined = new JObject();
            JObject jNewDataCombined = new JObject();

            // set old data
            _activities.ForEach(x =>
            {
                JObject values = JObject.FromObject(x.OldData);
                jOldDataCombined.Merge(values);
            });

            OldData = jOldDataCombined.ToObject<object>();

            // set new data
            _activities.ForEach(x =>
            {
                JObject values = JObject.FromObject(x.NewData);
                jNewDataCombined.Merge(values);
            });

            NewData = jNewDataCombined.ToObject<object>();
        }

        /// <summary>
        /// Returns true if collection contains items
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return _activities.Any();
        }
    }

    public class EntityActivity<TActivityType>
        where TActivityType : Enum
    {
        public object OldData { get; private set; }

        public object NewData { get; private set; }

        public EntityActivity(TActivityType activityType)
        {
            SetOldData(null, null);
            SetNewData(null, activityType, null);
        }

        public EntityActivity(TActivityType activityType, string propertyName,
            object originalPropertyValue, object newPropertyValue)
        {
            SetOldData(propertyName, originalPropertyValue);
            SetNewData(propertyName, activityType, newPropertyValue);
        }

        private void SetOldData(string propertyName, object originalPropertyValue)
        {
            // set old data
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                JObject jObjOld = new JObject();
                jObjOld.Add(propertyName, originalPropertyValue == null ? null : JToken.FromObject(originalPropertyValue));
                OldData = jObjOld.ToObject<object>();
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

            NewData = jObjNew.ToObject<object>();
        }
    }
}
