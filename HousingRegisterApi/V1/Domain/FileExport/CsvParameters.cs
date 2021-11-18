using System.Collections.Generic;

namespace HousingRegisterApi.V1.Domain.FileExport
{
    public class CsvParameters
    {
        /// <summary>
        /// Determines if headers should be included in the output.
        /// Defaults to true.
        /// </summary>
        public bool IncludeHeaders { get; set; }

        public CsvParameters()
        {
            IncludeHeaders = true;
        }
    }
}
