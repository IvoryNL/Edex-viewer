namespace DataCare.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UnionSet<T> : ISet<T>
    {
        private ISet<T>[] carrierSets;

        public UnionSet(IEnumerable<ISet<T>> carrierSets) : this(carrierSets.ToArray()) { }

        public UnionSet(params ISet<T>[] carrierSets)
        {
            this.carrierSets = carrierSets;
        }

        public bool Add(T item)
        {
            throw new NotSupportedException();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return (new HashSet<T>(carrierSets.SelectMany(set => set))).IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return (new HashSet<T>(carrierSets.SelectMany(set => set))).IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return (new HashSet<T>(carrierSets.SelectMany(set => set))).IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return (new HashSet<T>(carrierSets.SelectMany(set => set))).IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return (new HashSet<T>(carrierSets.SelectMany(set => set))).Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return (new HashSet<T>(carrierSets.SelectMany(set => set))).SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return carrierSets.Any(set => set.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            (new HashSet<T>(carrierSets.SelectMany(set => set))).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return (new HashSet<T>(carrierSets.SelectMany(set => set))).Count(); }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (new HashSet<T>(carrierSets.SelectMany(set => set))).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}