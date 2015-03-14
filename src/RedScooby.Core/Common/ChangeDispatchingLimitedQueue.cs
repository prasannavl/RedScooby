// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using RedScooby.Infrastructure.Framework;

namespace RedScooby.Common
{
    public sealed class ChangeDispatchingLimitedQueue<T> : ChangeNotifyingLimitedQueueBase<T>
    {
        public ChangeDispatchingLimitedQueue(int capacity = 16) : base(capacity) { }
        public ChangeDispatchingLimitedQueue(Queue<T> existingQueue, int capacity) : base(existingQueue, capacity) { }

        public override void HandleCollectionChange(NotifyCollectionChangedEventArgs args)
        {
            HandlePropertyChanges();
            var handler = GetCollectionChangedEventHandlerDelegate();
            if (handler != null)
            {
                DispatchHelper.Current.Run(() => handler(this, args));
            }
        }

        public override void HandlePropertyChanges()
        {
            var handler = GetPropertyChangedHandlerDelegate();
            if (handler != null)
            {
                DispatchHelper.Current.Run(h => h(this, new PropertyChangedEventArgs(CountString)), handler);
            }
        }
    }
}
