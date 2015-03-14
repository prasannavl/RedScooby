// Author: Prasanna V. Loganathar
// Created: 9:16 PM 23-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using RedScooby.Common;
using RedScooby.Converters;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework;
using RedScooby.Logging;
using RedScooby.Logging.Core;
using RedScooby.Models;
using RedScooby.Utilities;

namespace RedScooby.Startup
{
    public sealed class WinRtBootstrapper : BootstrapperBase
    {
        public const string ScaleConverterKey = "ScaleConverter";
        private HostApp hostApp;
        private LaunchActivatedEventArgs launchActivatedEventArgs;

        public WinRtBootstrapper(ServiceRegistrationInfo serviceRegistrationInfo)
            : base(serviceRegistrationInfo) { }

        public void SetStartupArguments(HostApp app, LaunchActivatedEventArgs e)
        {
            hostApp = app;
            launchActivatedEventArgs = e;
        }

        protected override void SetupCore()
        {
            base.SetupCore();
            App.Current.ApplicationExitDelegate = delegate(bool saveSave, bool restartIfPossible)
            {
                if (saveSave)
                {
                    try
                    {
                        // Application exiting. Possible for states to be in an impossible state to save.
                        // Wrap into nested try catch blocks to make sure we try each of the steps. One of those
                        // places where the code just can't be pretty.
                        try
                        {
                            App.Current.SaveCoreStates();
                        }
                        catch { }
                        App.Current.SaveAppModelAsync().ContinueWith(t => Application.Current.Exit());
                    }
                    catch
                    {
                        Application.Current.Exit();
                    }
                }
                else
                {
                    Application.Current.Exit();
                }
            };
        }

        [DebuggerNonUserCode]
        protected override void SetupLogging()
        {
            base.SetupLogging();
            LogManager.SetLevel(LogLevel.Trace);
            var _ = Task.Run(() => SetupFileSystemLogger());
        }

        protected override void SetupShell()
        {
            base.SetupShell();
            View.SetMessageDialogHelper(new WinRtMessageDialogHelper());

            if (!ModeDetector.IsInDesignMode)
            {
#if DEBUG
                hostApp.DebugSettings.IsBindingTracingEnabled = true;
                hostApp.DebugSettings.EnableFrameRateCounter = true;
                hostApp.DebugSettings.EnableRedrawRegions = false;
                hostApp.DebugSettings.IsOverdrawHeatMapEnabled = false;
#endif
                WorkaroundWinRTResources();
                SetupWinRtShell();
            }
        }

        protected override async Task StartAsync()
        {
            await base.StartAsync();

            if (launchActivatedEventArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore state.
            }

            ResolveStartupView();

            ShellHelper.ActivateMainWindowIfRequired(launchActivatedEventArgs);

            var _ = Task.Run(() => new TaskRegistrations().Run())
                .ContinueWithLogErrorOnException();
        }

        private async Task SetupFileSystemLogger()
        {
            await FileSystemLogger.CreateAsync();
            LogManager.Events.Subscribe(FileSystemLogger.Current);
        }

        private void SetupWinRtShell()
        {
            SetupExceptionHandlers();
            var di = DisplayInformation.GetForCurrentView();
            var scaler = (ScaleConverter) hostApp.Resources[ScaleConverterKey];
            scaler.Initialize(di);
        }

        private void ResolveStartupView()
        {
            var model = App.Current.Services.Locate<AppModel>();

            if (model.LocalSettings.IsFirstRun)
                ShellHelper.SwitchToWelcomePage(launchActivatedEventArgs);
            else if (model.User.Id == 0)
                ShellHelper.SwitchToSignupPage(launchActivatedEventArgs);
            if (!model.User.Session.IsAuthenticationValid)
                ShellHelper.SwitchToLoginPage(launchActivatedEventArgs);
        }

        private Action<Exception, string, bool> CreateExceptionHandlerDelegate()
        {
            return (ex, logPrefix, shouldQuit) => { };
        }

        private void SetupExceptionHandlers()
        {
            var errorHandler = new WinRtErrorHandler();
            ErrorHandler.Initialize(errorHandler);

            hostApp.UnhandledException +=
                (sender, args) => { errorHandler.HandleCrashException(args); };

            TaskScheduler.UnobservedTaskException +=
                (o, eventArgs) => { errorHandler.HandleUnobservedTaskException(eventArgs); };
        }

        private void WorkaroundWinRTResources()
        {
            var resType = typeof (ResourceComposition).GetTypeInfo();

            var types = resType.Assembly.ExportedTypes
                .Where(x => x.Namespace.StartsWith(resType.Namespace));

            types.ForEach(WinRtResourceManager.InjectIntoResxGeneratedApplicationResourcesClass);
        }
    }
}
