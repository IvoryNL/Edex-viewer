namespace DataCare.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public static class CollectionExtensions
    {
        public static void UpdateFrom<T>(this ICollection<T> changelist, IEnumerable<T> inputlist)
        {
            Contract.Requires(changelist != null);
            Contract.Requires(inputlist != null);

            var input = inputlist.ToList(); // Memoize the input list, as we want to go through it twice
            var itemsToAdd = input.Except(changelist).ToList();
            var itemsToRemove = changelist.Except(input).ToList();
            changelist.RemoveRange(itemsToRemove);
            changelist.AddRange(itemsToAdd);
        }

        public static int InsertBefore<T>(this Collection<T> list, T insertItem, IComparer<T> comparer)
        {
            Contract.Requires(list != null);
            int index = 0;
            while (index < list.Count && comparer.Compare(insertItem, list[index]) >= 0)
            {
                index++;
            }

            return index;
        }

        public static void UpdateFromOrdered<T>(this Collection<T> changelist, IEnumerable<T> inputlist, IComparer<T> comparer)
        {
            Contract.Requires(changelist != null);
            Contract.Requires(inputlist != null);

            var input = inputlist.ToList(); // Memoize the input list, as we want to go through it twice
            var itemsToAdd = input.Except(changelist).ToList();
            var itemsToRemove = changelist.Except(input).ToList();
            changelist.RemoveRange(itemsToRemove);
            itemsToAdd.ForEach(item =>
                {
                    changelist.Insert(changelist.InsertBefore(item, comparer), item);
                });
        }

        /// <summary>
        /// Synchroniseer collectie van type T en subtype U met een lijst van U
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="changelist">te synchroniseren collectie</param>
        /// <param name="inputlist">lijst waar tegenaan gesynchroniseerd moet worden</param>
        /// <param name="convert">functie om van een element T uit de te synchroniseren collectie tot subtype U gekomen kan worden</param>
        /// <param name="convertBack">functie om van subtype U weer het element T uit de te synchroniseren collectie te bepalen</param>
        /// <param name="conditionAdd">voorwaarde om een nieuw element T op basis van U te mogen toevoegen aan de collectie van T</param>
        /// <param name="conditionRemove">voorwaarde om een element T uit de collectie van T te mogen verwijderen</param>
        [Obsolete("Conversie en filtering moet niet duur genoeg zijn om dit nodig te maken")]
        public static void UpdateFrom<T, U>(
            this ICollection<T> changelist,
            IEnumerable<U> inputlist,
            Func<T, U> convert,
            Func<U, T> convertBack,
            Func<U, bool> conditionAdd,
            Func<T, bool> conditionRemove)
        {
            Contract.Requires(changelist != null);
            Contract.Requires(inputlist != null);
            Contract.Requires(convert != null);
            Contract.Requires(convertBack != null);
            Contract.Requires(conditionAdd != null);
            Contract.Requires(conditionRemove != null);

            var input = inputlist.ToList(); // Memoize the input list, as we want to go through it twice
            var itemsToAdd = input.Where(item => conditionAdd(item) && !changelist.Select(convert).Contains(item)).ToList();
            var itemsToRemove = changelist.Where(clitem => conditionRemove(clitem) && !input.Contains(convert(clitem))).ToList();
            changelist.RemoveRange(itemsToRemove);
            changelist.AddRange(itemsToAdd.Select(convertBack));
        }

        /// <summary>
        /// Synchroniseer collectie van T met subtype U met een lijst van U in de volgorde van de aangeboden lijst van U
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="changelist"></param>
        /// <param name="inputlist"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        /// <param name="conditionAdd"></param>
        /// <param name="conditionRemove"></param>
        [Obsolete("Conversie en filtering moet niet duur genoeg zijn om dit nodig te maken")]
        public static void UpdateFromOrdered<T, U>(
            this ICollection<T> changelist,
            IEnumerable<U> inputlist,
            Func<T, U> convert,
            Func<U, T> convertBack,
            Func<U, bool> conditionAdd,
            Func<T, bool> conditionRemove)
            where T : class
        {
            Contract.Requires(changelist != null);
            Contract.Requires(inputlist != null);
            Contract.Requires(convert != null);
            Contract.Requires(convertBack != null);

            // Memoize the input list, as we want to go through it twice
            var input = inputlist.ToList();

            var inputKeepAdd = input.Where(conditionAdd).GroupJoin(
                changelist,
                m => m,
                convert,
                (model, bm) => new { model, bm });

            var inputKeepRemove = changelist.GroupJoin(
                input,
                convert,
                m => m,
                (bm, model) => new { bm, model });

            // add existing binding models in order specified by model list, if binding model does not exist, create it for the corresponding model
            var itemsToAdd = inputKeepAdd.Select(ika => ika.bm.SingleOrDefault() ?? convertBack(ika.model)).ToList();

            // keep binding models which should normally be removed because they are not in model list, but for which the condition to remove the binding model evaluates to false
            var itemsToKeep = inputKeepRemove.Where(ikr => !ikr.model.Any() && !conditionRemove(ikr.bm)).Select(ikr => ikr.bm).ToList();
            changelist.Clear();
            changelist.AddRange(itemsToAdd);
            changelist.AddRange(itemsToKeep);
        }

        public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source)
        {
            Contract.Requires(destination != null);

            // NOOT: de ToArray() is noodzakelijk om exceptions te voorkomen als je een collectie aan zichzelf toevoegt
            if (source != null) { foreach (var item in source.ToArray()) { destination.Add(item); } }
        }

        public static void RemoveRange<T>(this ICollection<T> destination, IEnumerable<T> source)
        {
            Contract.Requires(destination != null);

            // NOOT: de ToArray() is noodzakelijk om exceptions te voorkomen als je een collectie uit zichzelf verwijdert
            if (source != null) { foreach (var item in source.ToArray()) { destination.Remove(item); } }
        }

        /// <summary>
        /// Wraps this object instance into an IEnumerable&lt;T&gt;
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the wrapped object.</typeparam>
        /// <param name="item"> The object to wrap.</param>
        /// <returns>
        /// An IEnumerable&lt;T&gt; consisting of a single item.
        /// </returns>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// Determines if the other collection contains the same elements, irrespective of their order or duplicate occurrences
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool SetEquals<T>(this IEnumerable<T> self, IEnumerable<T> other)
        {
            return SetEquals(self, other, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Determines if the other collection contains the same elements according to given comparer, irrespective of their order or duplicate occurrences
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool SetEquals<T>(this IEnumerable<T> self, IEnumerable<T> other, IEqualityComparer<T> comparer)
        {
            return new HashSet<T>(self, comparer).SetEquals(new HashSet<T>(other, comparer));
        }
    }
}