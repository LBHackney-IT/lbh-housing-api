using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;

namespace HousingRegisterApi.V1.Infrastructure
{
    // TODO: Use Hackney.Core NuGet package...

    /// <summary>
    /// Converter for DateTime objects because the default handling expects the data time string to alays be in a very specific
    /// format and will throw an exception if not.
    /// </summary>
    public class DynamoDbDateTimeConverter : IPropertyConverter
    {
        public static readonly string DATEFORMAT = "yyyy-MM-ddTHH\\:mm\\:ss.fffffffZ";

        public static string FormatDate(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString(DATEFORMAT);
        }

        public DynamoDBEntry ToEntry(object value)
        {
            if (null == value) return new DynamoDBNull();

            return new Primitive { Value = FormatDate((DateTime) value) };
        }

        public object FromEntry(DynamoDBEntry entry)
        {
            Primitive primitive = entry as Primitive;
            if (null == primitive) return (DateTime?) null;

            var dtString = primitive.Value.ToString();
            return DateTime.Parse(dtString, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }
    }
}
