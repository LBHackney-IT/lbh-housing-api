using System.Collections.Generic;

namespace HousingRegisterApi.V1.Domain.FileExport
{
    public class ExportFile
    {
        public string FileName { get; set; }

        public string FileMimeType { get; set; }

        public byte[] Data { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public ExportFile(string fileName, string fileMime, byte[] data)
        {
            FileName = fileName;
            FileMimeType = fileMime;
            Data = data;
            Metadata = new Dictionary<string, string>();
        }
    }
}
