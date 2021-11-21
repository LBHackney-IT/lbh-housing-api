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

            return result;
        }

        public static List<Tag> AppendTags(this Dictionary<string, string> source, List<Tag> tags)
        {
            var result = source.ToTagList();

            if (tags?.Any() == true)
            {
                // remove existing and replace with new ones
                result.RemoveAll(t => tags.Exists(x => x.Key.Equals(t.Key, StringComparison.OrdinalIgnoreCase)));
                result.AddRange(tags);
            }

            return result;
        }
    }
}
