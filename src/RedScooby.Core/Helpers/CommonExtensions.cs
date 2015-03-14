// Author: Prasanna V. Loganathar
// Created: 12:09 PM 22-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Text;

namespace RedScooby.Helpers
{
    public static class CommonExtensions
    {
        public static string ToConservativeTitleCase(this string source)
        {
            if (source == null)
                return null;
            if (source.Length == 0)
                return source;

            var result = new StringBuilder(source);
            result[0] = char.ToUpper(result[0]);
            for (var i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
                else
                    result[i] = result[i];
            }
            return result.ToString();
        }

        public static int FindIndex<T>(this IList<T> source, Predicate<T> match, int startIndex = 0)
        {
            var count = source.Count;
            if (count == 0 || startIndex >= count || startIndex < 0) return -1;

            for (var i = startIndex; i < source.Count; i++)
            {
                if (match(source[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int FindFirstIndexOf<T>(this IEnumerable<T> source, Predicate<T> match)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (match(item))
                    return index;

                index++;
            }

            return -1;
        }

        public static void EnsureInitialized<T>(this Lazy<T> source)
        {
            var _ = source.Value;
        }
    }
}
