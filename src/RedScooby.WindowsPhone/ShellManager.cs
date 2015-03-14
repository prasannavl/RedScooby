// Author: Prasanna V. Loganathar
// Created: 1:27 AM 18-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Phone.UI.Input;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using RedScooby.Helpers;
using RedScooby.Startup;
using RedScooby.Views;
using RedScooby.Views.Setup;

namespace RedScooby
{
    internal sealed class ShellManager
    {
        private readonly HostApp app;
        private DisplayRequest displayRequest;
        private SuspendingEventHandler suspendEventHandler;
        private EventHandler<object> resumeEventHandler;
        private bool appExiting;

        private ShellManager(HostApp app)
        {
            this.app = app;
            AddSuspendResumeHandlers();
        }

        public static ShellManager Current { get; private set; }

        public event EventHandler<BackPressedEventArgs> BackPressed
        {
            add
            {
                // Change order of handlers to make sure default handler is handled only in the end.
                HardwareButtons.BackPressed -= DefaultBackButtonHandler;
                HardwareButtons.BackPressed += value;
                HardwareButtons.BackPressed += DefaultBackButtonHandler;
            }

            remove { HardwareButtons.BackPressed -= value; }
        }

        public static void Create(HostApp app)
        {
            Current = new ShellManager(app);
        }

        public static WinRtBootstrapper CreateBootstrapper()
        {
            return new WinRtBootstrapper(new ServiceRegistrationInfo
            {
                Core = new WinRtCoreServiceRegistrar(),
                CoreComponents = new WinRtCoreComponentsRegistrar(),
                Components = new WinRtComponentsRegistrar(),
            });
        }

        public void PrepareForExitOnError()
        {
            appExiting = true;
            ClearSuspendResumeHandlers();
            app.Suspending += (sender, args) =>
            {
                args.SuspendingOperation.GetDeferral();
                App.Current.ApplicationExitDelegate.InvokeIfNotNullWith(true, false);
            };
        }

        public async Task HandleLaunchAsync(LaunchActivatedEventArgs e)
        {
            if (appExiting) App.Current.ApplicationExitDelegate.InvokeIfNotNullWith(true, false);

            // Temporary workaround for WinRT apps not running during lock screen.
            if (displayRequest == null) displayRequest = new DisplayRequest();
            displayRequest.RequestActive();

            if (e.PreviousExecutionState == ApplicationExecutionState.Running
                || e.PreviousExecutionState == ApplicationExecutionState.Suspended)
            {
                // App is already running. Handle secondary command parameters here.
                ShellHelper.ActivateMainWindowIfRequired(e);
            }
            else
            {
                var bootstrapper = CreateBootstrapper();
                bootstrapper.SetStartupArguments(app, e);
                await bootstrapper.RunAsync();

                SetupHardwareButtons();
            }
        }

        private void SetupHardwareButtons()
        {
            HardwareButtons.BackPressed += DefaultBackButtonHandler;
        }

        private void DefaultBackButtonHandler(object sender, BackPressedEventArgs e)
        {
            if (e.Handled) return;
            var frame = Window.Current.Content as Frame;
            if (frame != null && frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }

        private void ClearSuspendResumeHandlers()
        {
            app.Suspending -= suspendEventHandler;
            app.Resuming -= resumeEventHandler;
        }

        private void AddSuspendResumeHandlers()
        {
            if (suspendEventHandler == null)
                suspendEventHandler = (sender, args) => { var _ = SuspendAsync(args); };

            if (resumeEventHandler == null)
                resumeEventHandler = (sender, o) => { var _ = ResumeAsync(); };

            app.Suspending += suspendEventHandler;
            app.Resuming += resumeEventHandler;
        }

        private async Task ResumeAsync()
        {
            // Temporary workaround for WinRT apps not running during lock screen.
            displayRequest.RequestActive();
            await App.Current.ResumeAsync();
        }

        private async Task SuspendAsync(SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            displayRequest.RequestRelease();
            await App.Current.SuspendAsync();
            deferral.Complete();
        }
    }

    public class ShellHelper
    {
        public static void LoadMainPageIfRequired(LaunchActivatedEventArgs e, bool forceLoad = false)
        {
            var rootFrame = GetOrCreateRootFrame();

            if (rootFrame.Content == null || forceLoad)
            {
                if (!rootFrame.Navigate(typeof (MainView), e != null ? e.Arguments : null))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
        }

        public static void ActivateMainWindowIfRequired(LaunchActivatedEventArgs e)
        {
            if (Window.Current.Content == null)
            {
                LoadMainPageIfRequired(e);
                Window.Current.Activate();
            }
        }

        public static Frame GetOrCreateRootFrame()
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame {CacheSize = 1};
                Window.Current.Content = rootFrame;
            }
            return rootFrame;
        }

        public static void SwitchToLoginPage(LaunchActivatedEventArgs e)
        {
            var page = new UserSetupView();
            page.CarryLaunchParameters(e);
            Window.Current.Content = page;
            Window.Current.Activate();
        }

        public static void SwitchToSignupPage(LaunchActivatedEventArgs e)
        {
            var page = new UserSetupView(false, true);
            page.CarryLaunchParameters(e);
            Window.Current.Content = page;
            Window.Current.Activate();
        }

        public static void SwitchToWelcomePage(LaunchActivatedEventArgs e)
        {
            var page = new WelcomeView();
            page.InitializeForSplashScreen(e);
            page.FinishRender += () => { Window.Current.Activate(); };
            Window.Current.Content = page;
        }
    }
}
