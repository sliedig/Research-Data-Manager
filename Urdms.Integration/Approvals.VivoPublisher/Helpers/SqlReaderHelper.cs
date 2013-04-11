using System;
using System.Data.SqlClient;

namespace Urdms.Approvals.VivoPublisher.Helpers
{
    public static class SqlReaderHelper
    {
        public static T? GetNullableValue<T>(this SqlDataReader reader, string field) where T : struct
        {
            var ordinal = reader.GetOrdinal(field);
            if (reader.IsDBNull(ordinal))
            {
                return default(T?);
            }
            try
            {
                var value = reader.GetValue(ordinal);
                return (T)value;
            }
            catch
            {
                return default(T?);
            }
        }

        public static string GetStringValue(this SqlDataReader reader, string field, string defaultValueIfNullOrWhitespace = "N/A")
        {
            var ordinal = reader.GetOrdinal(field);
            if (ordinal < 0 || reader.IsDBNull(ordinal))
            {
                return defaultValueIfNullOrWhitespace;
            }
            var value = reader.GetString(ordinal);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValueIfNullOrWhitespace;
            }
            return value;
        }

        public static DateTime GetDateTimeValue(this SqlDataReader reader, string field, string defaultValueIfNullOrWhitespace = "N/A")
        {
            var ordinal = reader.GetOrdinal(field);
            if (ordinal < 0)
            {
                throw new InvalidOperationException(string.Format("Field '{0}' does not exist", field));
            }
            var value = reader.GetDateTime(ordinal);
            return value;
        }

        public static T GetValue<T>(this SqlDataReader reader, string field, T defaultValue = default(T)) where T : struct
        {
            var nullableValue = reader.GetNullableValue<T>(field);
            if (!nullableValue.HasValue)
            {
                return defaultValue;
            }
            return nullableValue.Value;
        }

        public static T GetEnumValue<T>(this SqlDataReader reader, string field, T defaultValue = default(T)) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException(string.Format("Type '{0}' must be Enum",typeof(T).Name));
            }
            return reader.GetValue(field, defaultValue);
        }
    }
}