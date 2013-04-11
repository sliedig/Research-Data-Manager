using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Urdms.DocumentBuilderService.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Retrieve the description on the enum, e.g.
        /// [Description("Bright Pink")]
        /// BrightPink = 2,
        /// Then when you pass in the enum, it will retrieve the description
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>A string representing the friendly name</returns>
        public static string GetDescription(this Enum en)
        {
            var t = en.GetType();
            var member = t.GetMember(en.ToString()).FirstOrDefault();
            if (member != null)
            {
                var attrib = member.GetCustomAttributes(typeof (DescriptionAttribute), false).Cast<DescriptionAttribute>().FirstOrDefault();
                if (attrib != null)
                {
                    return attrib.Description;
                }
            }
            return en.ToString();
        }


        public static IEnumerable<string> GetDescriptions(this Enum en, params Enum[] exclusions)
        {
            var t = en.GetType();
            if (!t.IsDefined(typeof(FlagsAttribute),false))
            {
                throw new InvalidOperationException();
            }
            var values = Enum.GetValues(t).Cast<Enum>().Except(exclusions ?? new Enum[] {});
            var p = from value in values
                    where en.HasFlag(value)
                    select value.GetDescription();

            return p.ToList();
        }


    }

}
