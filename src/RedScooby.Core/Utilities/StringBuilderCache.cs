// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Text;

namespace RedScooby.Utilities
{
    public static class StringBuilderCache
    {
        private const int stringBuilderMemoryCacheSize = 360;
        [ThreadStatic] private static StringBuilder t_cachedInstance;

        public static StringBuilder Acquire(int capacity = 16)
        {
            if (capacity <= stringBuilderMemoryCacheSize)
            {
                var cachedInstance = t_cachedInstance;
                if (cachedInstance != null)
                {
                    t_cachedInstance = null;
                    return cachedInstance;
                }
            }
            return new StringBuilder(capacity);
        }

        public static string GetStringAndRelease(StringBuilder sb)
        {
            var result = sb.ToString();
            Release(sb);
            return result;
        }

        public static void Release(StringBuilder sb)
        {
            if (sb.Capacity <= stringBuilderMemoryCacheSize)
            {
                sb.Length = 0;
                t_cachedInstance = sb;
            }
        }
    }
}
