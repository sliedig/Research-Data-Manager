using System;
using System.Collections.Generic;
using Curtin.Framework.Common.Extensions;

namespace Urdms.Dmp.Utils
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null || items.IsEmpty())
            {
                return;
            }
            foreach (var item in items)
            {
                list.Add(item);
            }
        }

        public static void RemoveAll<T>(this IList<T> list, Predicate<T> match)
        {
            if (match == null || list.IsEmpty())
            {
                return;
            }
            for (int i = list.Count - 1; i > -1; i--)
            {
                var entity = list[i];
                if (match(entity))
                {
                    list.RemoveAt(i);
                }
            }

        }
    }
}
