// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using RedScooby.Logging;

namespace RedScooby
{
    public sealed partial class HostApp : Application
    {
        public HostApp()
        {
            InitializeComponent();
            ShellManager.Create(this);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            ShellManager.Current.HandleLaunchAsync(args)
                .ContinueWith(t =>
                {
                    // Don't use regular error handler, to be able to log events
                    // even before the initialization of logging subsystem.
                    if (t.IsFaulted)
                    {
                        Log.Error(t.Exception);
                    }
                });
        }
    }
}
