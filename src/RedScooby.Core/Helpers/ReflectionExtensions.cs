// Author: Prasanna V. Loganathar
// Created: 1:33 AM 26-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RedScooby.Helpers
{
    public static class ReflectionExtensions
    {
        public static MethodInfo GetRuntimeMethodEx(this Type type, string name, params Type[] types)
        {
            var typesComparer = new TypesEqualityComparer();

            var method = type.GetRuntimeMethods()
                .Where(m => m.Name == name)
                .FirstOrDefault(m =>
                {
                    var typeParams = m.GetParameters()
                        .Select(p => p.ParameterType).ToArray();

                    return typeParams.Length == types.Length
                           && typeParams.SequenceEqual(types, typesComparer);
                });

            return method;
        }

        public class TypesEqualityComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type actual, Type expected)
            {
                if (actual == expected)
                    return true;

                if (actual.GetTypeInfo().IsGenericType && expected.GetTypeInfo().IsGenericTypeDefinition)
                    return actual.GetGenericTypeDefinition() == expected;

                return false;
            }

            public int GetHashCode(Type obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
