using System;
using System.Collections.Generic;

namespace Urdms.Approvals.VivoPublisher.Helpers
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null)
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
            if (match == null)
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