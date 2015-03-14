// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Infrastructure.Messaging
{
    public interface IMessenger : ICoreMessenger<object>
    {
        IDisposable SubscribeTo<T>(T value, Action<T> handler);
        IDisposable SubscribeTo<T>(T value, Action handler);
        IDisposable SubscribeTo<T>(Action<T> handler, bool includeInherited = false);
    }
}
