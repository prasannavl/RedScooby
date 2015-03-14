// Author: Prasanna V. Loganathar
// Created: 12:06 AM 19-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using RedScooby.Infrastructure.Messaging;
using RedScooby.Infrastructure.Scaffold;
using RedScooby.Utilities;

namespace RedScooby.Infrastructure.Composition
{
    public interface IViewModel : IDisposable, IChangeNotifyingObject
    {
        IMessenger Messenger { get; }
        IMessenger ViewMessenger { get; }
        Task ResumeAsync();
        Task SuspendAsync();
    }

    public static class ViewModel
    {
        static ViewModel()
        {
            Messenger = new ObjectMessenger();
        }

        public static IMessenger Messenger { get; private set; }

        public static void SetMessenger(IMessenger messenger)
        {
            Messenger = messenger;
        }
    }

    public class ViewModelBase : ChangeDispatchingObject, IViewModel
    {
        private IMessenger messenger = ViewModel.Messenger;
        private IMessenger viewMessenger = View.Messenger;

        public ViewModelBase(IMessenger messenger, IMessenger viewMessenger) : this()
        {
            Messenger = messenger;
            ViewMessenger = viewMessenger;
        }

        public ViewModelBase()
        {
            Disposables = new List<IDisposable>(1);
        }

        [IgnoreDataMember]
        protected List<IDisposable> Disposables { get; private set; }

        public virtual void Dispose()
        {
            foreach (var disposable in Disposables)
            {
                disposable.Dispose();
            }
        }

        public Task SuspendAsync()
        {
            return OnSuspendAsync();
        }

        public Task ResumeAsync()
        {
            return OnResumeAsync();
        }

        [IgnoreDataMember]
        public IMessenger Messenger
        {
            get { return messenger; }
            protected set { messenger = value; }
        }

        [IgnoreDataMember]
        public IMessenger ViewMessenger
        {
            get { return viewMessenger; }
            protected set { viewMessenger = value; }
        }

        public void ForwardMessageToView<T>(T value)
        {
            SubscribeTo(value, () => ViewMessenger.Send(value));
        }

        public void ForwardMessageToView<T>()
        {
            SubscribeTo<T>(msg => ViewMessenger.Send(msg));
        }

        public void ForwardMessageToView<T>(T value, Action action)
        {
            SubscribeTo(value, () =>
            {
                ViewMessenger.Send(value);
                action();
            });
        }

        public void ForwardMessageToView<T>(T value, Action<T> action)
        {
            SubscribeTo(value, () =>
            {
                ViewMessenger.Send(value);
                action(value);
            });
        }

        public void AddDisposable(IDisposable disposable)
        {
            Disposables.Add(disposable);
        }

        public IDisposable MapNotificationsFrom(INotifyPropertyChanged sender,
            KeyValuePair<string, string> senderToReceiverPropertyMap,
            bool autoDispose)
        {
            var handler = CreateAttachedHandler(sender, senderToReceiverPropertyMap);
            var disposable = Disposable.Create(() => sender.PropertyChanged -= handler);

            if (autoDispose) AddDisposable(disposable);
            return disposable;
        }

        public IDisposable MapNotificationsFrom(INotifyPropertyChanged sender,
            IEnumerable<KeyValuePair<string, string>> senderToReceiverPropertyMap, bool autoDispose)
        {
            var handler = CreateAttachedHandler(sender, senderToReceiverPropertyMap);
            var disposable = Disposable.Create(() => sender.PropertyChanged -= handler);

            if (autoDispose) AddDisposable(disposable);
            return disposable;
        }

        public void SubscribeTo<T>(T value, Action<T> handler)
        {
            AddDisposable(Messenger.SubscribeTo(value, handler));
        }

        public void SubscribeTo<T>(T value, Action handler)
        {
            AddDisposable(Messenger.SubscribeTo(value, handler));
        }

        public void SubscribeTo<T>(Action<T> handler, bool includeInherited = false)
        {
            AddDisposable(Messenger.SubscribeTo(handler, includeInherited));
        }

        protected virtual Task OnSuspendAsync()
        {
            return TaskCache.Completed;
        }

        protected virtual Task OnResumeAsync()
        {
            return TaskCache.Completed;
        }
    }
}
