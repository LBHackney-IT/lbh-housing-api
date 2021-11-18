using HousingRegisterApi.V1.Domain.FileExport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace HousingRegisterApi.V1.Services
{
    public class CSVGeneratorService : ICSVService
    {
        public async Task<byte[]> Generate(Array source, CsvParameters csvParameters)
        {
            var sourceType = source.GetType();

            return await Task.Run(() =>
            {
                return GenerateBytes(source, csvParameters);
            }).ConfigureAwait(false);
        }

        private static byte[] GenerateBytes(Array source, CsvParameters csvParameters)
        {
            var csvData = GetData(source);

            // convert data to bytes
            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms))
            {
                if (csvParameters.IncludeHeaders)
                {
                    sw.WriteLine(string.Join(",", csvData.Headers));
                }

                foreach (var dataRow in csvData.DataRows)
                {
                    sw.WriteLine(string.Join(",", dataRow));
                }
            };

            return ms.ToArray();
        }

        /// <summary>
        /// Returns the CsvData object
        /// </summary>
        /// <param name="array"></param>
        private static CsvData GetData(Array array)
        {
            var headers = GetHeaders(array.GetType().GetElementType());
            var rowsOfValues = new List<List<string>>();

            foreach (var entity in array)
            {
                rowsOfValues.Add(GetValues(entity));
            }

            return new CsvData()
            {
                Headers = headers,
                DataRows = rowsOfValues
            };
        }

        /// <summary>
        /// Returns the headers, based on the property names
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private static List<string> GetHeaders(Type entityType)
        {
            PropertyInfo[] properties = entityType.GetProperties();
            List<string> headers = new List<string>();

            foreach (var property in properties)
            {
                headers.Add(property.Name);
            }

            return headers;
        }

        /// <summary>
        /// Gets a row of values
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static List<string> GetValues(object source)
        {
            Type sourceType = source.GetType();
            PropertyInfo[] properties = sourceType.GetProperties();
            List<string> values = new List<string>();

            foreach (var property in properties)
            {
                values.Add(ConvertToString(property.GetValue(source)));
            }

            return values;
        }

        private static string ConvertToString(object dataItem)
        {
            if (dataItem == null)
            {
                return "";
            }

            string dataItemString = dataItem.ToString();

            if (dataItemString.Contains(","))
            {
                if (dataItemString.Contains("\""))
                {
                    dataItemString = dataItemString.Replace("\"", "\"\"");
                }

                dataItemString = $"\"{dataItemString}\"";
            }

            return dataItemString;
        }
    }
}
