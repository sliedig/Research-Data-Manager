using System;
using System.ComponentModel;
using System.Linq;

namespace Urdms.Dmp.Utils
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var field = type.GetField(value.ToString());
            var attrib = field.GetCustomAttributes(typeof (DescriptionAttribute), false).Cast<DescriptionAttribute>().FirstOrDefault();
            if (attrib == null || string.IsNullOrWhiteSpace(attrib.Description))
            {
                return value.ToString();
            }
            return attrib.Description;
        }

        public static T GetEnumValue<T>(this string value, T defaultValue = default(T)) 
        {
            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException("Only enums can be used here");
            }
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return defaultValue;
                }
                var result = (T) Enum.Parse(typeof (T), value, true);
                return result;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}