using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Domain.Report.Novalet
{
    public class NovaletFileAttributes
    {
        public DateTime? ApprovedOn { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? LastDownloadedOn { get; set; }
        public string LastDownloadedBy { get; set; }
        public DateTime? TransferredOn { get; set; }
    }

    public static class NovaletFileAttributesExtensions
    {
        public static Dictionary<string, string> ToDictionary(this NovaletFileAttributes source)
        {
            var json = JsonConvert.SerializeObject(source);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return dictionary;
        }

        public static NovaletFileAttributes ToObject(this Dictionary<string, string> source)
        {
            var json = JsonConvert.SerializeObject(source);
            var attributes = JsonConvert.DeserializeObject<NovaletFileAttributes>(json);
            return attributes;
        }
    }
}
