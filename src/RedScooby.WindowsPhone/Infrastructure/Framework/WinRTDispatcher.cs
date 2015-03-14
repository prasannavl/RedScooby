// Author: Prasanna V. Loganathar
// Created: 12:07 AM 19-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using RedScooby.Logging;

namespace RedScooby.Infrastructure.Framework
{
    public sealed class WinRtDispatcher : IDispatcher
    {
        private CoreDispatcher _dispatcher;

        public void Initialize()
        {
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            Scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        public bool CheckAccess()
        {
            return _dispatcher.HasThreadAccess;
        }

        public void Run(Action action)
        {
            if (CheckAccess())
                action();
            else
            {
                Dispatch(action);
            }
        }

        public void Dispatch(Action action)
        {
            try
            {
                _dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(action)).Completed +=
                    (info, status) =>
                    {
                        if (status == AsyncStatus.Error)
                        {
                            ErrorHandler.Current.HandleError(info.ErrorCode);
                        }
                    };
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void Dispatch<T>(Action<T> action, T state)
        {
            try
            {
                _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action(state)).Completed +=
                    (info, status) =>
                    {
                        if (status == AsyncStatus.Error)
                        {
                            ErrorHandler.Current.HandleError(info.ErrorCode);
                        }
                    };
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void Run<T>(Action<T> action, T state)
        {
            if (CheckAccess()) { }
            else
            {
                Dispatch(action, state);
            }
        }

        public TaskScheduler Scheduler { get; private set; }
    }
}
