using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.V1.Infrastructure
{
    public static class TaggingExtensions
    {
        public static Dictionary<string, string> ToDictionary(this List<Tag> source)
        {
            var result = new Dictionary<string, string>();

            if (source?.Any() == true)
            {
                source.ForEach(x =>
                {
                    result.Add(x.Key, x.Value);
                });
            }

            return result;
        }

        public static Dictionary<string, string> ToDictionary(this Tagging source)
        {
            var result = new Dictionary<string, string>();

            if (source?.TagSet?.Any() == true)
            {
                result = source.TagSet.ToDictionary();
            }

            return result;
        }

        public static List<Tag> ToTagList(this Dictionary<string, string> source)
        {
            var result = new List<Tag>();

            if (source?.Any() == true)
            {
                foreach (var meta in source)
                {
                    result.Add(new Tag
                    {
                        Key = meta.Key,
                        Value = meta.Value
                    });
                }
            }

            // remove all attributes where there are null or empty strings
            // otherwise the tag update request will fail
            result.RemoveAll(x => string.IsNullOrWhiteSpace(x.Value));
            return result;
        }

        public static List<Tag> AppendAttributes(this List<Tag> tags, Dictionary<string, string> attributes)
        {
            var destination = tags.ToDictionary();

            if (attributes?.Any() == true)
            {
                foreach (var attribute in attributes)
                {
                    if (destination.ContainsKey(attribute.Key))
                    {
                        // overwrite values
                        destination[attribute.Key] = attribute.Value;
                    }
                    else
                    {
                        // add new value
                        destination.Add(attribute.Key, attribute.Value);
                    }
                }
            }

            return destination.ToTagList();
        }
    }
}
