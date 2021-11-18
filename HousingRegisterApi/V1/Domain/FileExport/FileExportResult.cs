using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Domain.FileExport
{
    public class FileExportResult
    {
        public string FileName { get; set; }

        public string FileMimeType { get; set; }

        public byte[] Data { get; set; }

        public FileExportResult(string fileName, string fileMime, byte[] data)
        {
            FileName = fileName;
            FileMimeType = fileMime;
            Data = data;
        }
    }
}
