// -----------------------------------------------------------------------
// <copyright file="Range.cs" company="DataCare BV">
// </copyright>
// -----------------------------------------------------------------------

namespace DataCare.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public static class Range
    {
        public static Range<T> Create<T>(T firstBound, T secondBound) where T : IComparable<T>
        {
            return new Range<T>(firstBound, secondBound);
        }
    }

    /// <summary>
    /// Simple class representing a range of numbers
    /// </summary>
    public class Range<T> where T : IComparable<T>
    {
        public T LowerBound { get; private set; }
        public T UpperBound { get; private set; }

        public Range(T firstBound, T secondBound)
        {
            var comparison = firstBound.CompareTo(secondBound);
            if (comparison <= 0)
            {
                LowerBound = firstBound;
                UpperBound = secondBound;
            }
            else
            {
                UpperBound = firstBound;
                LowerBound = secondBound;
            }
        }

        public bool Contains(T value)
        {
            return LowerBound.CompareTo(value) <= 0 && UpperBound.CompareTo(value) >= 0;
        }

        public bool Contains(Range<T> range)
        {
            Contract.Requires(range != null);

            return LowerBound.CompareTo(range.LowerBound) <= 0 && UpperBound.CompareTo(range.UpperBound) >= 0;
        }

        public bool IsContainedBy(Range<T> range)
        {
            Contract.Requires(range != null);

            return range.Contains(this);
        }

        public bool Overlaps(Range<T> range)
        {
            Contract.Requires(range != null);

            return Contains(range.LowerBound) || Contains(range.UpperBound) || IsContainedBy(range);
        }

        public IEnumerable<T> Step(Func<T, T> next)
        {
            for (T current = LowerBound; current.CompareTo(UpperBound) <= 0; current = next(current))
            {
                yield return current;
            }
        }
    }
}