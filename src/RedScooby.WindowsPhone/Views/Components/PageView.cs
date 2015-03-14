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
    public class PageBase : Page
    {
        protected bool IsModelInitialized;
        protected object ModelContext;

        public PageBase()
        {
            DesignComponent.InitializeApp();
        }

        public void SetModelContext(object context)
        {
            ModelContext = context;
            if (IsModelInitialized)
            {
                OnSetModelContext(context);
            }
        }

        protected virtual void OnSetModelContext(object context) { }
    }

    public class PageView<TViewModel> : PageBase, IView<TViewModel> where TViewModel : IViewModel
    {
        private IMessenger viewMessenger = View.Messenger;
        private TViewModel model;

        public PageView()
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

        public PageView(IMessenger messenger)
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
                OnSetModelContext(ModelContext);
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

        public void HideAppBar()
        {
            BottomAppBar = null;
            // Workaround to make sure the layout doesn't get messed up if the hiding is slow enough to not take place before the switching on low-end devices.
            var ignore = Dispatcher.RunIdleAsync(x => { UpdateLayout(); });
        }

        public void ShowAppBar(AppBar appBar)
        {
            BottomAppBar = appBar;
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
