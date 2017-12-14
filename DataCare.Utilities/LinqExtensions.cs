// -----------------------------------------------------------------------
// <copyright file="LinqExtensions.cs" company="Data Care BV">
// </copyright>
// -----------------------------------------------------------------------

namespace DataCare.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// </summary>
    public static class LinqExtensions
    {
        public static IEnumerable<TResult> Scan<TInput, TResult>(
            this IEnumerable<TInput> input,
            TResult seed,
            Func<TResult, TInput, TResult> accumulate)
        {
            yield return seed;
            foreach (var item in input)
            {
                seed = accumulate(seed, item);
                yield return seed;
            }
        }

        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(
            IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }

            if (second == null)
            {
                throw new ArgumentNullException("second");
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }

            return ZipIterator(first, second, resultSelector);
        }

        private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            using (IEnumerator<TFirst> firstEnumerator = first.GetEnumerator())
            using (IEnumerator<TSecond> secondEnumerator = second.GetEnumerator())
            {
                while (firstEnumerator.MoveNext() && secondEnumerator.MoveNext())
                {
                    yield return resultSelector(firstEnumerator.Current, secondEnumerator.Current);
                }
            }
        }

        public static IEnumerable<V> LookupOrDefault<K, V>(this ILookup<K, V> lookup, K key)
        {
            if (lookup != null)
            {
                if (lookup.Contains(key))
                {
                    return lookup[key];
                }

                return Enumerable.Empty<V>();
            }

            return Enumerable.Empty<V>();
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int items)
        {
            for (int i = 0; i < items; ++i)
            {
                yield return source.Where((x, index) => (index % items) == i);
            }
        }
    }
}