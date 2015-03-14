// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using RedScooby.Infrastructure.Framework;

namespace RedScooby.Infrastructure.Messaging
{
    //TODO 1: Rewrite messages with immutable data structures.
    //TODO 2: Try to implement generic message passing to remove the overhead of boxing.
    public class ObjectMessenger : CoreMessenger<object>, IDisposable, IMessenger
    {
        private readonly IScheduler scheduler;
        private readonly Dictionary<Type, List<Action<object>>> concreteSubscriptions;
        private readonly Action<object> globalSubscriptionAction;
        private IDisposable globalSubscription;
        private volatile bool isSubscribed;

        public ObjectMessenger(IScheduler scheduler)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            concreteSubscriptions = new Dictionary<Type, List<Action<object>>>();
            globalSubscriptionAction = (m =>
            {
                var type = m.GetType();
                List<Action<object>> x;
                if (concreteSubscriptions.TryGetValue(type, out x))
                {
                    //REF: TODO 1 - Unnecessary allocations
                    foreach (var handler in x.ToArray())
                    {
                        try
                        {
                            // Caution: Async void lamdas will be thrown on to the UnhandledExceptionHandler
                            handler(m);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.Current.HandleError(ex);
                        }
                    }
                }
            });

            this.scheduler = scheduler;
        }

        public ObjectMessenger()
            : this(DefaultScheduler.Instance) { }

        void IDisposable.Dispose()
        {
            if (globalSubscription != null)
                globalSubscription.Dispose();

            Subject.Dispose();
        }

        public IDisposable SubscribeTo<T>(T value, Action<T> handler)
        {
            var handleWrapper = new Action<object>(o =>
            {
                var current = (T) o;
                if (value.Equals(current))
                    handler(current);
            });

            return SubscribeInternal<T>(handleWrapper);
        }

        public IDisposable SubscribeTo<T>(T value, Action handler)
        {
            var handleWrapper = new Action<object>(o =>
            {
                var current = (T) o;
                if (value.Equals(current))
                    handler();
            });

            return SubscribeInternal<T>(handleWrapper);
        }

        public IDisposable SubscribeTo<T>(Action<T> handler, bool includeInherited = false)
        {
            if (includeInherited)
            {
                return Messages
                    .ObserveOn(scheduler)
                    .Subscribe(m =>
                    {
                        // Warning: Multiple casts. Since there is no way around without using a class contraint on T.  
                        // Inheritance tree walk is much more expensive than an 'is' IL instruction.
                        if (m is T)
                        {
                            handler((T) m);
                        }
                    });
            }
            var handleWrapper = new Action<object>(o => handler((T) o));
            return SubscribeInternal<T>(handleWrapper);
        }

        private IDisposable SubscribeInternal<T>(Action<object> handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            var type = typeof (T);
            List<Action<object>> x;
            if (!concreteSubscriptions.TryGetValue(type, out x))
            {
                x = new List<Action<object>>();
                concreteSubscriptions.Add(type, x);
            }
            x.Add(handler);

            if (!isSubscribed)
            {
                isSubscribed = true;
                globalSubscription = Messages
                    .ObserveOn(scheduler)
                    .Subscribe(globalSubscriptionAction);
            }

            return Disposable.Create(() =>
            {
                x.Remove(handler);
                if (x.Count == 0)
                    concreteSubscriptions.Remove(type);
            });
        }
    }
}
