// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive;
using System.Threading.Tasks;

namespace RedScooby.Utilities
{
    public static class EmptyInstanceCache<TResult> where TResult : new()
    {
        public static TResult Instance = new TResult();
    }

    public static class TaskCache
    {
        public static Task<bool> False = Task.FromResult(false);
        public static Task<bool> True = Task.FromResult(true);
        public static Task Completed = True;
    }

    public static class TaskCache<TResult> where TResult : struct
    {
        public static Task<TResult> Default = Task.FromResult(default(TResult));
    }

    public static class TaskNullCache<TResult> where TResult : class
    {
        public static Task<TResult> Null = Task.FromResult<TResult>(null);
    }

    public static class TaskEmptyInstanceCache<TResult> where TResult : class, new()
    {
        public static Task<TResult> Instance = Task.FromResult(EmptyInstanceCache<TResult>.Instance);
    }

    public static class DelegatesCache
    {
        public static Action EmptyAction = () => { };
        public static Action<Unit> EmptyUnit = x => { };
    }
}
