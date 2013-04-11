using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;

namespace Urdms.Dmp.Tests.Helpers
{
    internal static class PickHelper
    {
        public static IList<TEnum> GetEnumValues<TEnum>(params TEnum[] exclusions) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException("Type is not an enum");
            }
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            var list = values.Except(exclusions ?? new TEnum[] { }).ToList();
            return list;
        }

        public static TEnum RandomEnumExcept<TEnum>(params TEnum[] exclusions) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException("Type is not an enum");
            }
            var list = GetEnumValues(exclusions);
            return Pick<TEnum>.RandomItemFrom(list);
        }

        public static bool RandomBoolean()
        {
            return Pick<bool>.RandomItemFrom(new[] {false, true});
        }

        public static TEnumFlag RandomEnumsExcept<TEnumFlag>(params TEnumFlag[] exclusions) where TEnumFlag : struct
        {
            if (!typeof(TEnumFlag).IsEnum)
            {
                throw new InvalidOperationException("Type is not an enum");
            }
            if (!typeof(TEnumFlag).IsDefined(typeof(FlagsAttribute),false))
            {
                throw new InvalidOperationException("Type is not an enum decorated with the FlagsAttribute");
            }
            var list = GetEnumValues(exclusions);
            var selected = new List<TEnumFlag>();
            if (list.Count <= 2)
            {
                selected.AddRange(list);
            }
            else
            {
                while(selected.Count < 2)
                {
                    var pickedItem = Pick<TEnumFlag>.RandomItemFrom(list.Except(selected).ToList());
                    selected.Add(pickedItem);
                }
                
            }
            int value = 0;
            foreach(var item in selected.Cast<int>())
            {
                value |= item;
            }
            return (TEnumFlag)(object)value;

        }
    }
}
