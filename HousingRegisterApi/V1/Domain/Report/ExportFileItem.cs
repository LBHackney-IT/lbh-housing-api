using System;
using System.Collections.Generic;

namespace HousingRegisterApi.V1.Domain.Report
{
    public class ExportFileItem
    {
        public string FileName { get; set; }

        public DateTime LastModified { get; set; }

        public long Size { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public ExportFileItem(string fileName)
        {
            FileName = fileName;
            Attributes = new Dictionary<string, string>();
        }
    }
}
