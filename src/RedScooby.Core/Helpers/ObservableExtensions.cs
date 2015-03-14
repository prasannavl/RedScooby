// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Specialized;
using System.Reactive.Linq;

namespace RedScooby.Helpers
{
    public static class ObservableExtensions
    {
        public static IObservable<NotifyCollectionChangedEventArgs> FromCollectionChangedEvent(
            this INotifyCollectionChanged observableCollection)
        {
            return Observable
                .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    handler => (o, e) => handler(e),
                    handler => observableCollection.CollectionChanged += handler,
                    handler => observableCollection.CollectionChanged -= handler);
        }
    }
}
