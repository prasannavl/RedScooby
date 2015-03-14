// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Diagnostics;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using RedScooby.Api;
using RedScooby.Api.Core;
using RedScooby.Data.Core;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework;
using RedScooby.Logging;
using RedScooby.Logging.Core;
using RedScooby.Logging.FormatProvider;
using RedScooby.Logging.Observers;
using RedScooby.Models;
using RedScooby.ViewModels;

namespace RedScooby.Startup
{
    public abstract class BootstrapperBase
    {
        private readonly ServiceRegistrationInfo serviceRegistrationInfo;
        private App app;
        private DependencyInjectionContainer container;
        private AppModel model;
        private bool loggingInitialized;
        private bool coreInitialized;
        private bool coreComponentsInitialized;
        private bool appModelInitialized;
        private bool componentsInitialized;
        private bool apiComponentsInitialized;
        private bool extensionsInitialized;
        private bool shellInitialized;

        protected BootstrapperBase(ServiceRegistrationInfo serviceRegistrationInfo)
        {
            this.serviceRegistrationInfo = serviceRegistrationInfo;
        }

        public async Task RunAsync()
        {
            InitializeLogging();
            InitializeCore();
            await InitializeAppModelAsync();
            InitializeCoreComponents();
            InitializeComponents();
            InitializeApiComponents();
            InitializeExtensions();
            InitializeShell();
            await StartAsync();
        }

        [DebuggerNonUserCode]
        public void InitializeLogging(bool force = false)
        {
            if (!ModeDetector.IsInDesignMode)
            {
                if (loggingInitialized && !force) return;
                SetupLogging();
                loggingInitialized = true;
            }
        }

        public void InitializeCore(bool force = false)
        {
            if (coreInitialized && !force) return;
            SetupCore();
            coreInitialized = true;
        }

        public async Task InitializeAppModelAsync(bool force = false)
        {
            if (appModelInitialized && !force) return;
            await SetupAppModelAsync();
            appModelInitialized = true;
        }

        public void InitializeCoreComponents(bool force = false)
        {
            if (coreComponentsInitialized && !force) return;
            SetupCoreComponents();
            coreComponentsInitialized = true;
        }

        public void InitializeComponents(bool force = false)
        {
            if (componentsInitialized && !force) return;
            SetupComponents();
            componentsInitialized = true;
        }

        public void InitializeExtensions(bool force = false)
        {
            if (extensionsInitialized && !force) return;
            SetupExtensions();
            extensionsInitialized = true;
        }

        public void InitializeApiComponents(bool force = false)
        {
            if (apiComponentsInitialized && !force) return;
            SetupApiComponents();
            apiComponentsInitialized = true;
        }

        public void InitializeShell(bool force = false)
        {
            if (shellInitialized && !force) return;
            SetupShell();
            shellInitialized = true;
        }

        protected virtual void SetupLogging()
        {
            LogManager.SetLogger(new ObservableLogger(
                new DumpFormatProvider()));
#if DEBUG
            LogManager.SetLevel(LogLevel.Trace);

            LogManager.Events.Subscribe(new DebugWriter());
#else
            LogManager.SetLevel(LogLevel.Error);
#endif
        }

        protected virtual void SetupCore()
        {
            app = new App();

            container = serviceRegistrationInfo.CreateContainer();
            serviceRegistrationInfo.Core.Register(container);
            app.Services = container;
            App.Current = app;

            DispatchHelper.Current.Initialize();
        }

        protected virtual void SetupCoreComponents()
        {
            serviceRegistrationInfo.CoreComponents.Register(container, model);
            // View is initialized in the core so that the correct context for messaging is set. View initialization does nothing else, but just support view messenger. It is harmless, since there will really be no subscribers without the view.
            View.Initialize();
        }

        protected virtual async Task SetupAppModelAsync()
        {
            model = container.Locate<AppModel>();
            var store = (IAsyncStoreFoundation) model;
            await store.InitializeStoreAsync();
            await model.LoadAllAsync();
        }

        protected virtual void SetupComponents()
        {
            serviceRegistrationInfo.Components.Register(container, model);
        }

        protected virtual void SetupExtensions()
        {
            serviceRegistrationInfo.Extensions.ForEachIfNotNull(x => x.Register(container, model));
        }

        protected virtual void SetupShell()
        {
            ViewModelLocator.Scope = container.RootScope;
        }

        protected virtual void SetupApiComponents()
        {
            var apiConfig = new RedScoobyApiConfiguration("", "");
            RedScoobyApi.Initialize(apiConfig);
        }

        protected virtual Task StartAsync()
        {
            return App.Current.StartAsync();
        }
    }
}
