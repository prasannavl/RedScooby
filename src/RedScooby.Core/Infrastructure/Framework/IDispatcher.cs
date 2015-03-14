// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;

namespace RedScooby.Infrastructure.Framework
{
    public interface IDispatcher
    {
        TaskScheduler Scheduler { get; }
        bool CheckAccess();
        void Dispatch(Action action);
        void Dispatch<T>(Action<T> action, T state);
        void Initialize();
        void Run(Action action);
        void Run<T>(Action<T> action, T state);
    }
}
