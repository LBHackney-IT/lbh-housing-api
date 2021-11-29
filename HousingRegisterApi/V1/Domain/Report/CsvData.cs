using System.Collections.Generic;

namespace HousingRegisterApi.V1.Domain.Report
{
    public class CsvData
    {
        public List<string> Headers = new List<string>();
        public List<List<string>> DataRows = new List<List<string>>();
    }
}
