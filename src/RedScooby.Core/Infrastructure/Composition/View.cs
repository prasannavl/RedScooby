// Author: Prasanna V. Loganathar
// Created: 12:06 AM 19-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using RedScooby.Infrastructure.Framework;
using RedScooby.Infrastructure.Messaging;
using RedScooby.Utilities;
using RedScooby.ViewModels;

namespace RedScooby.Infrastructure.Composition
{
    public interface IView<out TViewModel> where TViewModel : IViewModel
    {
        TViewModel Model { get; }
        IMessenger ViewMessenger { get; }
        TViewModel CreateViewModel();
    }

    public static class View
    {
        private static IMessenger _messenger;
        private static IMessageDialogHelper _messageDialogHelper;

        public static IMessageDialogHelper MessageDialogHelper
        {
            get
            {
                if (_messageDialogHelper == null)
                    throw new InvalidOperationException("MessageDialogHelper has not been set");
                return _messageDialogHelper;
            }
            private set { _messageDialogHelper = value; }
        }

        public static IMessenger Messenger
        {
            get
            {
                if (_messenger == null) Initialize();
                return _messenger;
            }
            private set { _messenger = value; }
        }

        public static void SetMessageDialogHelper(IMessageDialogHelper helper)
        {
            if (_messageDialogHelper != null) throw new Exception("MessageDialogHelper can only be set once.");
            MessageDialogHelper = helper;
        }

        public static void Initialize()
        {
            ThrowIfInitialized();
            Messenger = new ObjectMessenger(new SynchronizationContextScheduler(SynchronizationContext.Current));
        }

        public static void Initialize(IScheduler scheduler)
        {
            ThrowIfInitialized();
            Messenger = new ObjectMessenger(scheduler);
        }

        public static void Initialize(SynchronizationContext context)
        {
            ThrowIfInitialized();
            Messenger = new ObjectMessenger(new SynchronizationContextScheduler(context));
        }

        private static void ThrowIfInitialized()
        {
            if (_messenger != null) throw new Exception("Already initialized.");
        }
    }

    public abstract class ViewBase<TViewModel> : IView<TViewModel> where TViewModel : IViewModel
    {
        protected bool IsModelInitialized;
        private IMessenger viewMessenger = View.Messenger;
        private TViewModel model;

        protected ViewBase()
        {
            VolatileDisposables = new List<IDisposable>(1);
        }

        protected ViewBase(IMessenger messenger)
            : this()
        {
            viewMessenger = messenger;
        }

        protected List<IDisposable> VolatileDisposables { get; private set; }

        public virtual TViewModel CreateViewModel()
        {
            return ViewModelLocator.Scope.Locate<TViewModel>();
        }

        [IgnoreDataMember]
        public IMessenger ViewMessenger
        {
            get { return viewMessenger; }
            protected set { viewMessenger = value; }
        }

        public virtual TViewModel Model
        {
            get { return model; }
            set
            {
                if (Equals(model, value)) return;

                model = value;
                if (value != null)
                    AddVolatileDisposable(value);
                IsModelInitialized = true;
            }
        }

        public void SubscribeTo<T>(T value, Action<T> handler)
        {
            AddVolatileDisposable(ViewMessenger.SubscribeTo(value, handler));
        }

        public void SubscribeTo<T>(T value, Action handler)
        {
            AddVolatileDisposable(ViewMessenger.SubscribeTo(value, handler));
        }

        public void SubscribeTo<T>(Action<T> handler, bool includeInherited = false)
        {
            AddVolatileDisposable(ViewMessenger.SubscribeTo(handler, includeInherited));
        }

        public void AddVolatileDisposable(IDisposable disposable)
        {
            VolatileDisposables.Add(disposable);
        }

        protected virtual void OnInitialized()
        {
            if (!IsModelInitialized)
                Model = CreateViewModel();
        }

        protected virtual void OnLoaded()
        {
            AddSuspendHandler();

            if (!IsModelInitialized)
                Model = CreateViewModel();
        }

        protected virtual void OnUnloaded()
        {
            foreach (var disposable in VolatileDisposables)
            {
                if (disposable != null)
                    disposable.Dispose();
            }

            VolatileDisposables.Clear();
            IsModelInitialized = false;

            RemoveSuspendHandler();
        }

        protected virtual Task OnSuspendAsync()
        {
            AddResumeHandler();
            return Model != null ? Model.SuspendAsync() : TaskCache.Completed;
        }

        protected virtual Task OnResumeAsync()
        {
            RemoveResumeHandler();
            return Model != null ? Model.ResumeAsync() : TaskCache.Completed;
        }

        protected void AddSuspendHandler()
        {
            App.Current.Suspending += OnSuspendAsync;
        }

        protected void RemoveSuspendHandler()
        {
            App.Current.Suspending -= OnSuspendAsync;
        }

        protected void AddResumeHandler()
        {
            App.Current.Resuming += OnResumeAsync;
        }

        protected void RemoveResumeHandler()
        {
            App.Current.Resuming -= OnResumeAsync;
        }
    }
}
