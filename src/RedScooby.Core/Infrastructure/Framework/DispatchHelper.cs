// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using RedScooby.Views.Components;

namespace RedScooby.Infrastructure.Framework
{
    public static class DispatchHelper
    {
        static DispatchHelper()
        {
            if (!DesignComponent.IsActive)
            {
#if NETFX_CORE
                Current = new WinRtDispatcher();
#else 
                Current = new SynchronizationContextDispatcher();
#endif
            }
            else
            {
                Current = new SynchronizationContextDispatcher();
            }
        }

        public static IDispatcher Current { get; private set; }
    }

    public static class DispatcherExtensions
    {
        public static Task RunAsync(this IDispatcher dispatcher, Func<Task> task)
        {
            return dispatcher.CheckAccess() ? task() : dispatcher.DispatchAsync(task);
        }

        public static Task DispatchAsync(this IDispatcher dispatcher, Func<Task> task)
        {
            return Task.Factory.StartNew(task,
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                dispatcher.Scheduler).Unwrap();
        }
    }
}
