// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedScooby.Infrastructure.Framework
{
    public class SynchronizationContextDispatcher : IDispatcher
    {
        private static SynchronizationContext _uiContext;

        public void Run<T>(Action<T> action, T state)
        {
            if (CheckAccess())
            {
                action(state);
            }
            else
            {
                Dispatch(action, state);
            }
        }

        public void Initialize()
        {
            _uiContext = SynchronizationContext.Current;

            if (_uiContext == null)
            {
                throw new Exception("Synchronization context unavailable.");
            }

            Scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        public void Dispatch(Action action)
        {
            _uiContext.Post(o => action(), null);
        }

        public void Dispatch<T>(Action<T> action, T state)
        {
            _uiContext.Post(s => action((T) s), state);
        }

        public bool CheckAccess()
        {
            return SynchronizationContext.Current == _uiContext;
        }

        public void Run(Action action)
        {
            if (CheckAccess())
            {
                action();
            }
            else
            {
                Dispatch(action);
            }
        }

        public TaskScheduler Scheduler { get; private set; }
    }
}
