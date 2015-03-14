// Author: Prasanna V. Loganathar
// Created: 9:20 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Messaging;
using RedScooby.Utilities;
using RedScooby.ViewModels;

namespace RedScooby.Views.Components
{
    public class ContentControlBase : ContentControl
    {
        public ContentControlBase()
        {
            DesignComponent.InitializeApp();
        }
    }

    public class ContentControlView<TViewModel> : ContentControlBase, IView<TViewModel> where TViewModel : IViewModel
    {
        protected bool IsModelInitialized;
        private IMessenger viewMessenger = View.Messenger;
        private TViewModel model;

        public ContentControlView()
        {
            VolatileDisposables = new List<IDisposable>(1);
            RoutedEventHandler h = null;
            h = (sender, args) =>
            {
                Loaded -= h;
                OnInitialized();
            };

            Loaded += h;
            Loaded += (sender, args) => { OnLoaded(); };
            Unloaded += (sender, args) => { OnUnloaded(); };
        }

        public ContentControlView(IMessenger messenger)
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
                DataContext = model;
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
