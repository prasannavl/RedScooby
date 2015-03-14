// Author: Prasanna V. Loganathar
// Created: 9:24 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using RedScooby.Components;
using RedScooby.Helpers;

namespace RedScooby.Views.Setup
{
    public sealed partial class WelcomeView : Page
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        public event Action FinishRender;

        public void InitializeForSplashScreen(LaunchActivatedEventArgs e)
        {
            var splash = e.SplashScreen;

            StartupImage.SetValue(WidthProperty, splash.ImageLocation.Width);
            StartupImage.SetValue(HeightProperty, splash.ImageLocation.Height);

            StartupImage.ImageOpened += (sender, args) =>
            {
                var handler = FinishRender;
                if (handler != null) handler.Invoke();
            };

            Loaded += async (sender, args) =>
            {
                // Warm up tasks for login / registration components during the animations.
                var sb = StatusBar.GetForCurrentView();
                await sb.HideAsync();
                var story = SplashToHelloStory.RunAsync();

                UserSetupView userSetupView = null;

                var warmup = Task.Delay(1000).ContinueWith(async t =>
                {
                    var tcs = new TaskCompletionSource<bool>();
                    var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
                    {
                        using (ComponentsHelper.AddUserSetupScope())
                        {
                            // XAML Loader loads asynchronously, which in turn loads the UserSetupViewModel retriving the services in the provided scope.
                            userSetupView = new UserSetupView(false, true);
                            await userSetupView.InitializeAsync();
                        }
                        userSetupView.CarryLaunchParameters(e);
                        tcs.TrySetResult(true);
                    });

                    await tcs.Task;
                });

                await story;
                var tsx = sb.ShowAsync();
                await warmup;
                await tsx;

                Window.Current.Content = userSetupView;
            };
        }
    }
}
