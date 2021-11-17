using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class EntityActivity<TActivityType>
        where TActivityType : Enum
    {
        public TActivityType ActivityType { get; private set; }

        public object OldData { get; private set; }

        public object NewData { get; private set; }

        public EntityActivity(TActivityType activityType)
        {
            ActivityType = activityType;
            SetOldData(null, null);
            SetNewData(null, activityType, null);
        }

        public EntityActivity(TActivityType activityType, string propertyName,
            object originalPropertyValue, object newPropertyValue)
        {
            ActivityType = activityType;
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
                OldData = JsonConvert.DeserializeObject(jObjOld.ToString());
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

            NewData = JsonConvert.DeserializeObject(jObjNew.ToString());
        }
    }
}
