namespace DataCare.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AccumulatingSet<T> : ISet<T>
    {
        private readonly ISet<T> baseSet;
        private readonly ISet<T> additions;

        public AccumulatingSet(ISet<T> baseSet)
        {
            this.baseSet = baseSet;
            this.additions = new HashSet<T>();
        }

        public bool Add(T item)
        {
            if (baseSet.Contains(item))
            {
                return false;
            }
            else
            {
                return additions.Add(item);
            }
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
            return new HashSet<T>(baseSet.Concat(additions)).IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return new HashSet<T>(baseSet.Concat(additions)).IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return baseSet.IsSubsetOf(other) && additions.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return new HashSet<T>(baseSet.Concat(additions)).IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return baseSet.Overlaps(other) || additions.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return new HashSet<T>(baseSet.Concat(additions)).SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            additions.UnionWith(other);
        }

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return baseSet.Contains(item) || additions.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            new HashSet<T>(baseSet.Concat(additions)).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return baseSet.Count + additions.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return baseSet.Concat(additions).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}