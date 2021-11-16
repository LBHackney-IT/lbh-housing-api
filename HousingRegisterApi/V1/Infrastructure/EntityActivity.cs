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
        public object OldData()
        {
            JObject jAllValues = new JObject();

            _activities.ForEach(x =>
            {
                JObject values = JObject.FromObject(x.OldData);
                jAllValues.Merge(values);
            });

            return jAllValues.ToObject<object>();
        }

        /// <summary>
        /// Combines all new data changes into a single object
        /// </summary>
        /// <returns></returns>
        public object NewData()
        {
            JObject jAllValues = new JObject();

            _activities.ForEach(x =>
            {
                JObject values = JObject.FromObject(x.NewData);
                jAllValues.Merge(values);
            });

            return jAllValues.ToObject<object>();
        }

        public EntityActivityCollection()
        {
            _activities = new List<EntityActivity<TActivityType>>();
        }

        public void Add(EntityActivity<TActivityType> activity)
        {
            _activities.Add(activity);
        }

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

        public EntityActivity(string propertyName, object originalPropertyValue, NewEntityActivity<TActivityType> newPropertyValue)
        {      
            JObject jObjOld = new JObject();
            jObjOld.Add(propertyName, JToken.FromObject(originalPropertyValue));

            JObject jObjNewValue = new JObject();
            jObjNewValue.Add(propertyName, JToken.FromObject(newPropertyValue.NewPropertyValue));

            JObject jObjNew = new JObject();
            jObjNew.Add(new JProperty("type", newPropertyValue.ActivityType));
            jObjNew.Add(new JProperty("payload", jObjNewValue));

            OldData = jObjOld.ToObject<object>();
            NewData = jObjNew.ToObject<object>();
        }
    }

    public class NewEntityActivity<T>
        where T : Enum
    {
        public T ActivityType { get; private set; }

        public object NewPropertyValue { get; private set; }

        public NewEntityActivity(T activityType, object newPropertyValue)
        {
            ActivityType = activityType;
            NewPropertyValue = newPropertyValue;
        }
    }
}
