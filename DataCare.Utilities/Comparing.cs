// -----------------------------------------------------------------------
// <copyright file="Comparing.cs" company="">
// </copyright>
// -----------------------------------------------------------------------

namespace DataCare.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// </summary>
    public static class Comparing
    {
        public static IEqualityComparer<T> EqualityOn<T, TKey>(Func<T, TKey> selector)
        {
            return new ComparingEqualityOn<T, TKey>(selector);
        }

        private sealed class ComparingEqualityOn<T, TKey> : EqualityComparer<T>
        {
            private readonly Func<T, TKey> selector;

            public ComparingEqualityOn(Func<T, TKey> selector)
            {
                Contract.Ensures(selector == this.selector);

                this.selector = selector;
            }

            public override bool Equals(T x, T y)
            {
                Contract.Ensures(this.selector != null);

                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                {
                    return false;
                }

                return Equals(this.selector(x), this.selector(y));
            }

            public override int GetHashCode(T obj)
            {
                return this.selector(obj).GetHashCode();
            }
        }

        public static IComparer<T> On<T, TKey>(Func<T, TKey> selector)
        {
            return new ComparingOn<T, TKey>(selector, Comparer<TKey>.Default);
        }

        public static IComparer<T> On<T, TKey>(Func<T, TKey> selector, Comparer<TKey> comparer)
        {
            return new ComparingOn<T, TKey>(selector, comparer);
        }

        private sealed class ComparingOn<T, TKey> : Comparer<T>
        {
            private readonly Func<T, TKey> selector;
            private readonly Comparer<TKey> comparer;

            public ComparingOn(Func<T, TKey> selector, Comparer<TKey> comparer)
            {
                Contract.Ensures(this.comparer == null);
                Contract.Ensures(selector == this.selector);

                this.selector = selector;
                this.comparer = comparer;
            }

            public override int Compare(T x, T y)
            {
                Contract.Ensures(this.selector != null);
                Contract.Ensures(this.comparer != null);

                return this.comparer.Compare(this.selector(x), this.selector(y));
            }
        }
    }
}