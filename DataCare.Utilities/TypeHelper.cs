// -----------------------------------------------------------------------
// <copyright file="TypeHelper.cs" company="DataCare BV">
// </copyright>
// -----------------------------------------------------------------------

namespace DataCare.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A class with extension methods on Type to make things easier when using reflection
    /// </summary>
    public static class TypeHelper
    {
        public static IEnumerable<Type> GetBases(this Type type)
        {
            var baseType = type.BaseType;

            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }

        private static Dictionary<string, Type> typeDict = new Dictionary<string, Type>();

        public static Type FindType(string typetag)
        {
            Type foundType;
            if (!typeDict.TryGetValue(typetag, out foundType))
            {
                foundType = Type.GetType(typetag, false) ??
                   AppDomain.CurrentDomain
                       .GetAssemblies()
                       .SelectMany(assembly => assembly.SafelyGetTypesMatching(type => type.Name == typetag))
                       .FirstOrDefault();
                typeDict.Add(typetag, foundType);
            }

            return foundType;
        }

        public static IEnumerable<Type> SafelyGetTypesMatching(this Assembly assembly, Predicate<Type> predicate)
        {
            ICollection<Type> types;
            try
            {
                types = assembly.GetTypes()
                    .Where(t => t != null && t.Assembly == assembly && predicate(t))
                    .ToList();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = new List<Type>();
                foreach (var type in ex.Types)
                {
                    try
                    {
                        if (type != null && type.Assembly == assembly && predicate(type))
                        {
                            types.Add(type);
                        }
                    }
                    catch (BadImageFormatException)
                    {
                        // Type was not in this assembly, ignore it
                    }
                }
            }

            return types;
        }

        public static IList<Type> GetArgumentsOfInterface(this Type type, Type genericInterface)
        {
            var typeWithArgument = type.GetInterfaces().FirstOrDefault(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericInterface);

            return (typeWithArgument != null)
                ? typeWithArgument.GetGenericArguments()
                : null;
        }
    }
}