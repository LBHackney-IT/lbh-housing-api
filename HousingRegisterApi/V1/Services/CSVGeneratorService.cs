using HousingRegisterApi.V1.Domain.FileExport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Services
{
    public class CSVGeneratorService : ICSVService
    {
        public async Task<byte[]> Generate<T>(T source, CsvParameters csvParameters)
        {
            var sourceType = source.GetType();
            var array = new List<T>();

            if (!sourceType.IsArray)
            {
                array.Add(source);
            }
            else
            {
                array = source as List<T>;
            }

            return await Task.Run(() =>
            {
                return GenerateBytes(array, csvParameters);
            }).ConfigureAwait(false);
        }

        private static byte[] GenerateBytes<T>(IList<T> source, CsvParameters csvParameters)
        {
            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms))
            {
                GetData<T>(source, out List<string> headers, out List<List<object>> rowsOfValues);

                if (csvParameters.IncludeHeaders)
                {
                    sw.WriteLine(string.Join(",", headers));
                }

                foreach (var itemRow in rowsOfValues)
                {
                    sw.WriteLine(string.Join(",", itemRow));
                }
            };

            return ms.ToArray();
        }

        private static void GetData<T>(IList<T> array, out List<string> headers, out List<List<object>> rowsOfValues)
        {
            headers = GetHeaders(typeof(T));
            rowsOfValues = new List<List<object>>();

            foreach (var entity in array)
            {
                rowsOfValues.Add(GetValues(entity));
            }
        }

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

        private static List<object> GetValues<T>(T source)
        {
            Type sourceType = source.GetType();
            PropertyInfo[] properties = sourceType.GetProperties();
            List<object> values = new List<object>();

            foreach (var property in properties)
            {
                values.Add(property.GetValue(source));
            }

            return values;
        }
    }

}
