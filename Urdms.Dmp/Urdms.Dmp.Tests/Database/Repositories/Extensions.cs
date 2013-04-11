using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Curtin.Framework.Common.Extensions;

namespace Urdms.Dmp.Tests.Database.Repositories
{
    public static class Extensions
    {
        public static Expression<Func<T, T, bool>> AndAlso<T>(this Expression<Func<T, T, bool>> left, Expression<Func<T, T, bool>> right)
        {
            var parameters = new[]
                                 {
                                     Expression.Parameter(typeof (T), "x"),
                                     Expression.Parameter(typeof (T), "y")
                                 };
            var body = Expression.AndAlso(
                    Expression.Invoke(left, parameters),
                    Expression.Invoke(right, parameters)
                );
            var lambda = Expression.Lambda<Func<T, T, bool>>(body, parameters);
            return lambda;
        }

        public static IEnumerable<T> IntersectByComparer<T>(this IEnumerable<T> source, IEnumerable<T> other, Func<T, T, bool> comparer)
        {
            if (source.IsEmpty() || other.IsEmpty())
            {
                yield break;
            }
            foreach (var item in source)
            {
                if (other.Any(o => comparer(item, o)))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> ExceptByComparer<T>(this IEnumerable<T> source, IEnumerable<T> other, Func<T, T, bool> comparer)
        {
            if (source.IsEmpty())
            {
                return Enumerable.Empty<T>();
            }
            if (other.IsEmpty())
            {
                return source;
            }
            var list = source.Where(o => !other.Any(q => comparer(o, q))).ToList();
            return list;

        }

        public static IEnumerable<T> NonIntersectByComparer<T>(this IEnumerable<T> source, IEnumerable<T> other, Func<T, T, bool> comparer)
        {
            var firstSet = source.ExceptByComparer(other, comparer);
            var secondSet = other.ExceptByComparer(source, comparer);
            var list = firstSet.Union(secondSet).ToList();
            return list;
        }
    }
}
