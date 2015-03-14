// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Helpers
{
    public static class DelegateExtensions
    {
        public static void InvokeIfNotNull(this Action action)
        {
            var handler = action;
            if (handler != null)
                handler();
        }

        public static void InvokeIfNotNull(this Action action, Func<bool> predicate)
        {
            var handler = action;
            if (handler != null && predicate != null && predicate())
                handler();
        }

        public static void InvokeIfNotNullWith<T>(this Action<T> action, T arg)
        {
            var handler = action;
            if (handler != null)
                handler(arg);
        }

        public static void InvokeIfNotNullWith<T>(this Action<T> action, T arg, Func<bool> predicate)
        {
            var handler = action;
            if (handler != null && predicate != null && predicate())
                handler(arg);
        }

        public static void InvokeIfNotNullWith<TArg1, TArg2>(this Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2)
        {
            var handler = action;
            if (handler != null)
                handler(arg1, arg2);
        }

        public static void InvokeIfNotNullWith<TArg1, TArg2>(this Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2,
            Func<bool> predicate)
        {
            var handler = action;
            if (handler != null && predicate != null && predicate())
                handler(arg1, arg2);
        }

        public static void InvokeIfNotNullWith<TArg, TArg2, TArg3>(this Action<TArg, TArg2, TArg3> action, TArg arg1,
            TArg2 arg2, TArg3 arg3)
        {
            var handler = action;
            if (handler != null)
                handler(arg1, arg2, arg3);
        }

        public static void InvokeIfNotNullWith<TArg, TArg2, TArg3>(this Action<TArg, TArg2, TArg3> action, TArg arg1,
            TArg2 arg2, TArg3 arg3, Func<bool> predicate)
        {
            var handler = action;
            if (handler != null && predicate != null && predicate())
                handler(arg1, arg2, arg3);
        }
    }
}
