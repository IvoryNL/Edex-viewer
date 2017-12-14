// -----------------------------------------------------------------------
// <copyright file="Hashing.cs" company="DataCare BV">
// </copyright>
// -----------------------------------------------------------------------

namespace DataCare.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// General utilities
    /// </summary>
    public static class Hashing
    {
        private static int Hash(int h, int v)
        {
            // See 'Performance in Practice of String Hashing Functions'
            // by   M.V. Ramakrishna and Justin Zobel
            // in   Proceedings of the Fifth International Conference on
            //      Database Systems for Advanced Applications
            return h ^ ((h << 7) + v + (h >> 2));
        }

        /// <summary>
        /// This computes hash codes for any enumerable. Note that it may not
        /// be the most efficient method; should this prove to be the case
        /// then extra utilities working on arrays can be easily added.
        /// </summary>
        /// <remarks>
        /// Do not use this for public facing hash tables, as with O(n) bucket
        /// access this will lead to denial of service attacks. See e.g.
        /// http://www.nruns.com/_downloads/advisory28122011.pdf
        /// </remarks>
        /// <typeparam name="T">The type of elements in the enumerable</typeparam>
        /// <param name="from">The elements to calculate a hash for</param>
        /// <returns>A hash code</returns>
        public static int CombineHashCodes<T>(this IEnumerable<T> from)
        {
            int constHash = 0x61E04917; // magic number slurped from .Net internals
            if (from == null)
            {
                return constHash;
            }

            return from.Aggregate(constHash, (h, k) => Hash(h, EqualityComparer<T>.Default.GetHashCode(k)));
        }

        public static int CombineHashCodes(this IEnumerable from)
        {
            int constHash = 0x61E04917; // magic number slurped from .Net internals
            int result = constHash;
            if (from == null)
            {
                return result;
            }

            foreach (var obj in from)
            {
                result = Hash(result, obj.GetHashCode());
            }

            return result;
        }
    }
}