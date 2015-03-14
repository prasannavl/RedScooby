// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Helpers
{
    public static class DisposableHelpers
    {
        public static IDisposable Create<T>(Action<T> action, T state)
        {
            if (action == null) throw new ArgumentNullException("action");
            return new DisposableImpl<T>(action, state);
        }

        private class DisposableImpl<T> : IDisposable
        {
            private readonly T state;
            private readonly Action<T> action;

            public DisposableImpl(Action<T> action, T state)
            {
                this.state = state;
                this.action = action;
            }

            public void Dispose()
            {
                action(state);
            }
        }
    }
}
