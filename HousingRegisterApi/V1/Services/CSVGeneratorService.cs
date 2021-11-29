using HousingRegisterApi.V1.Domain.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Services
{
    public class CsvGeneratorService : ICsvService
    {
        /// <summary>
        /// Generate CSV using default configuration
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public async Task<byte[]> Generate(Array source)
        {
            return await Generate(source, new CsvParameters()).ConfigureAwait(false);
        }

        /// <summary>
        /// Generate CSV with custom configuration
        /// </summary>
        /// <param name="source"></param>
        /// <param name="csvParameters"></param>
        /// <returns></returns>
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
            byte[] output = null;

            // convert data to bytes
            using (MemoryStream ms = new MemoryStream())
            {
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

                output = ms.ToArray();
            }
            return output;
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
                var description = property.GetCustomAttribute<DescriptionAttribute>();

                if (description != null)
                {
                    headers.Add(description.Description);
                }
                else
                {
                    headers.Add(property.Name);
                }
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
