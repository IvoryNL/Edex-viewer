namespace DataCare.Utilities
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class MemberHelper
    {
        public static string GetSelectedMemberName<TFrom, T>(Expression<Func<TFrom, T>> memberExpression)
        {
            return GetSelectedMember(memberExpression).Name;
        }

        public static MemberInfo GetSelectedMember<TFrom, T>(Expression<Func<TFrom, T>> memberExpression)
        {
            return GetSelectedMemberExpression(memberExpression).Member;
        }

        public static string GetSelectedMemberName<T>(Expression<Func<T>> memberExpression)
        {
            return GetSelectedMember(memberExpression).Name;
        }

        public static MemberInfo GetSelectedMember<T>(Expression<Func<T>> memberExpression)
        {
            return GetSelectedMemberExpression(memberExpression).Member;
        }

        private static MemberExpression GetSelectedMemberExpression<T>(Expression<T> memberSelector)
        {
            MemberExpression selectedMember;

            if (memberSelector == null)
            {
                throw new ArgumentNullException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Member selection expression can not be null"),
                    "memberSelector");
            }

            if (memberSelector.Body is MemberExpression)
            {
                selectedMember = memberSelector.Body as MemberExpression;
            }
            else
            {
                var body = memberSelector.Body as UnaryExpression;
                if (body != null)
                {
                    selectedMember = body.Operand as MemberExpression;
                    if (selectedMember == null)
                    {
                        throw new ArgumentException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Selector expression needs to be of the form '() => Member', '() => obj.Member' or '(obj) => obj.Member' but expression is: {0}",
                                memberSelector),
                            "memberSelector");
                    }
                }
                else
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Selector expression needs to be of the form '() => Member', '() => obj.Member' or '(obj) => obj.Member' but expression is: {0}",
                            memberSelector),
                        "memberSelector");
                }
            }

            return selectedMember;
        }

        internal static FieldInfo GetBackingField(PropertyInfo info)
        {
            // Very brittle implementation of GetBackingField (will probably
            // work on auto generated properties on MS C# compiler 4.0)

            // backing field is field on type declaring property
            if (info.DeclaringType == null)
            {
                return null;
            }

            return info.DeclaringType

                // backing field is a nonpublic field; we don't support static properties
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(
                    field =>
                    field.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any()

                    // backing field has field name in angle brackets (in MSC# 4.0)
                    && field.Name.Contains("<" + info.Name + ">")

                    // backing field has k__BackingField appended to it (in MSC# 4.0)
                    && field.Name.Contains("k__BackingField"));
        }
    }
}