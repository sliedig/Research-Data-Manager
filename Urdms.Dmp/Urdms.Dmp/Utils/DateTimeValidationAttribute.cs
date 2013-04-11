using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Urdms.Dmp.Utils
{
    /// <summary>
    /// To validate date time strings when a valid content is made available
    /// </summary>
    public class DateTimeValidationAttribute : ValidationAttribute
    {
        private static readonly string[] DefaultDatePatterns = new[] {"d/M/yyyy", "dd/MM/yyyy"};
        public string DatePatterns { get; set; }

        public DateTimeValidationAttribute() : base("Please enter a valid date")
        {}

        /// <summary>
        /// Validates the string to ensure that it is parseable to the DateTime object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            var text = (string) value;
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.Trim();
                var patterns = !string.IsNullOrWhiteSpace(DatePatterns)
                                   ? DatePatterns.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                   : DefaultDatePatterns;
                var culture = new CultureInfo("en-AU");
                DateTime result;
                return DateTime.TryParseExact(text, patterns, culture.DateTimeFormat, DateTimeStyles.None, out result);
            }
            return true;
        }

    }
}