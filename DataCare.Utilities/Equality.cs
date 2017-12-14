namespace DataCare.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class Equality
    {
        public static bool Given<T>(T one, T other, Func<T, T, bool> predicate) where T : class
        {
            if (ReferenceEquals(one, other))
            {
                return true;
            }

            // one^ != other^
            if (one == null || other == null)
            {
                return false;
            }

            return predicate(one, other);
        }

        public static bool Of<T>(T one, object other) where T : IEquatable<T>
        {
            return other is T && one.Equals((T)other);
        }

        /// <summary>
        /// bepaal de sleutelvelden van T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>enumerable van sleutelvelden</returns>
        private static IEnumerable<PropertyInfo> KeyProperties<T>()
        {
            Contract.Ensures(Contract.Result<PropertyInfo>() != null);

            return KeyProperties(typeof(T));
        }

        private static IEnumerable<PropertyInfo> AllProperties<T>()
        {
            Contract.Ensures(Contract.Result<PropertyInfo>() != null);

            return AllProperties(typeof(T));
        }

        private static IEnumerable<PropertyInfo> KeyProperties(Type type)
        {
            if (type.TypeHandle.Equals(typeof(object).TypeHandle))
            {
                yield break;
            }

            var typeKeyProperties = type.GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);

            foreach (var property in typeKeyProperties)
            {
                yield return property;
            }

            foreach (var property in KeyProperties(type.BaseType))
            {
                yield return property;
            }
        }

        private static IEnumerable<PropertyInfo> AllProperties(Type type)
        {
            if (type.TypeHandle.Equals(typeof(object).TypeHandle))
            {
                yield break;
            }

            foreach (var property in type.GetProperties())
            {
                yield return property;
            }

            foreach (var property in AllProperties(type.BaseType))
            {
                yield return property;
            }
        }

        /// <summary>
        /// determine lambda expression which evaluates equality between two objects of type T based on the attributed key field properties
        /// </summary>
        /// <example>Gegevens properties van T: sleutelveld: key_1, sleutelveld: key_2, dataveld: prop_3
        /// lambda: (p1,p2) =&gt;   (((object)p1) != null &amp;&amp; ((object)p2) != null))
        ///                       &amp;&amp; (   (((object)p1) == ((object)p2))
        ///                                   || (true &amp;&amp; p1.key_1 == p2.key_1 &amp;&amp; p1.key_2 == p2.key_2)
        /// </example>
        /// <typeparam name="T">Type van de sleutelhoudende objecten</typeparam>
        /// <returns>true als de objecten gelijk zijn op alle sleutelvelden, false anders.</returns>
        public static Expression<Func<T, T, bool>> KeyEquals<T>()
        {
            // definieer de parameters one en other in de lambda expressie
            ParameterExpression one = Expression.Parameter(typeof(T), "one");
            ParameterExpression other = Expression.Parameter(typeof(T), "other");

            Expression nonNullCheck =
                Expression.AndAlso(
                    Expression.ReferenceNotEqual(one, Expression.Constant(null)),
                    Expression.ReferenceNotEqual(other, Expression.Constant(null)));

            Expression referenceEqualityCheck =
                Expression.ReferenceEqual(one, other);

            var keyProperties = KeyProperties<T>().ToList();
            Expression keyEqualityChecks =
                keyProperties
                    .Aggregate(
                        nonNullCheck,
                        (predicate, property) =>
                        Expression.AndAlso(
                            predicate,
                            EqualityExpressionForProperty(one, other, property)));

            Expression equalityPredicate =
                keyProperties.Any()
                    ? Expression.OrElse(
                        referenceEqualityCheck,
                        keyEqualityChecks)
                    : referenceEqualityCheck;

            // genereer lambda expressie voor predicaat
            var lambda = Expression.Lambda<Func<T, T, bool>>(equalityPredicate, one, other);
            return lambda;
        }

        /// <summary>
        /// determine lambda expression which evaluates equality between two objects of type T based on the attributed key field properties
        /// </summary>
        /// <example>Gegevens properties van T: sleutelveld: key_1, sleutelveld: key_2, dataveld: prop_3
        /// lambda: (p1,p2) =&gt;   (((object)p1) != null &amp;&amp; ((object)p2) != null))
        ///                       &amp;&amp; (   (((object)p1) == ((object)p2))
        ///                                   || (true &amp;&amp; p1.key_1 == p2.key_1 &amp;&amp; p1.key_2 == p2.key_2)
        /// </example>
        /// <typeparam name="T">Type van de sleutelhoudende objecten</typeparam>
        /// <returns>true als de objecten gelijk zijn op alle sleutelvelden, false anders.</returns>
        public static Expression<Func<T, T, bool>> ValueEquals<T>()
        {
            // definieer de parameters one en other in de lambda expressie
            ParameterExpression one = Expression.Parameter(typeof(T), "one");
            ParameterExpression other = Expression.Parameter(typeof(T), "other");

            Expression nonNullCheck =
                Expression.AndAlso(
                    Expression.ReferenceNotEqual(one, Expression.Constant(null)),
                    Expression.ReferenceNotEqual(other, Expression.Constant(null)));

            Expression referenceEqualityCheck =
                Expression.ReferenceEqual(one, other);

            var allProperties = AllProperties<T>().ToList();
            Expression keyEqualityChecks =
                allProperties
                    .Aggregate(
                        nonNullCheck,
                        (predicate, property) =>
                        Expression.AndAlso(
                            predicate,
                            EqualityExpressionForProperty(one, other, property)));

            Expression equalityPredicate =
                allProperties.Any()
                    ? Expression.OrElse(
                        referenceEqualityCheck,
                        keyEqualityChecks)
                    : referenceEqualityCheck;

            // genereer lambda expressie voor predicaat
            var lambda = Expression.Lambda<Func<T, T, bool>>(equalityPredicate, one, other);
            return lambda;
        }

        private static Expression EqualityExpressionForProperty(ParameterExpression one, ParameterExpression other, PropertyInfo property)
        {
            Contract.Requires(one != null);
            Contract.Requires(other != null);
            Contract.Requires(property != null);

            if (property.PropertyType.IsValueType)
            {
                return
                    Expression.Equal(
                        Expression.Property(one, property.Name),
                        Expression.Property(other, property.Name));
            }

            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
            {
                MethodInfo sequenceEqual = typeof(Equality).GetMethod("SequenceEqual", BindingFlags.Static | BindingFlags.NonPublic);
                return
                    Expression.Call(
                        sequenceEqual,
                        Expression.Property(one, property.Name),
                        Expression.Property(other, property.Name));
            }

            MethodInfo objectEquals = typeof(object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);

            // (one.property != null && one.property.Equals(other.property))
            var propertyEquality = Expression.AndAlso(
                        Expression.NotEqual(Expression.Property(one, property.Name), Expression.Constant(null)),
                        Expression.Call(objectEquals, Expression.Property(one, property.Name), Expression.Property(other, property.Name)));

            // (one.property == null && other.property == null)
            var propertyNullity = Expression.AndAlso(
                        Expression.Equal(Expression.Property(one, property.Name), Expression.Constant(null)),
                        Expression.Equal(Expression.Property(other, property.Name), Expression.Constant(null)));

            // (one.property != null && one.property.Equals(other.property)) || (one.property == null && other.property == null)
            return Expression.OrElse(propertyEquality, propertyNullity);
        }

        // NOTE: This is used by refelection in line 211
        private static bool SequenceEqual(IEnumerable first, IEnumerable second)
        {
            if (first == null)
            {
                return second == null;
            }

            if (second == null)
            {
                return false;
            }

            var firstEnumerator = first.GetEnumerator();
            var secondEnumerator = second.GetEnumerator();
            while (firstEnumerator.MoveNext())
            {
                if (!secondEnumerator.MoveNext())
                {
                    return false;
                }

                if ((firstEnumerator.Current != null && !firstEnumerator.Current.Equals(secondEnumerator.Current))
                    || (firstEnumerator.Current == null && secondEnumerator.Current != null))
                {
                    return false;
                }
            }

            return !secondEnumerator.MoveNext();
        }

        public static int GetHashCodeKey<T>(T entity)
        {
            Contract.Requires(!ReferenceEquals(entity, null));

            PropertyInfo[] keyProps = KeyProperties<T>().ToArray();

            return keyProps.Any()
                ? GetHashCodeArray(entity, new ArraySegment<PropertyInfo>(keyProps, 0, keyProps.Length))
                : entity.GetHashCode();
        }

        private static int GetHashCodeArray<T>(T entity, ArraySegment<PropertyInfo> segment)
        {
            Contract.Requires(!ReferenceEquals(entity, null));
            //// CodeContracts: Suggested requires:: Contract.Requires(((!(segment.Count) || segment.Count == 1) || (segment.Offset + 1) < segment.Array.Length));
            //// CodeContracts: Suggested requires: Contract.Requires(((!(segment.Count) || segment.Count != 1) || segment.Offset < segment.Array.Length));

            return GetTupleForArray(entity, segment).GetHashCode();
        }

        private static object GetTupleForArray<T>(T entity, ArraySegment<PropertyInfo> segment)
        {
            Contract.Requires(!ReferenceEquals(entity, null), "entity is null");
            Contract.Requires(segment != null, "segment is null");
            Contract.Requires(segment.Count == 0 || segment.Count == 1 || segment.Offset + 1 < segment.Array.Length);
            Contract.Requires(segment.Count != 1 || segment.Offset < segment.Array.Length);
            ////Contract.Requires(((!(segment.Count) || segment.Count != 1 ) || segment.Offset < segment.Array.Length)); // suggested code contract that does not make sence

            switch (segment.Count)
            {
                case 0:
                    return 0;

                case 1:
                    return Tuple.Create(GetHashInput(segment.Array[segment.Offset + 0], entity));

                default:
                    return Tuple.Create(
                        GetHashInput(segment.Array[segment.Offset + 0], entity),
                        GetHashInput(segment.Array[segment.Offset + 1], entity),
                        GetTupleForArray(entity, new ArraySegment<PropertyInfo>(segment.Array, segment.Offset + 2, segment.Count - 2)));
            }
        }

        private static object GetHashInput(PropertyInfo info, object entity)
        {
            Contract.Requires(info != null);

            return typeof(IEnumerable).IsAssignableFrom(info.PropertyType) && info.PropertyType != typeof(string)
                       ? ((IEnumerable)info.GetValue(entity, null)).CombineHashCodes()
                       : info.GetValue(entity, null);
        }
    }
}