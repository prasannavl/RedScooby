// Author: Prasanna V. Loganathar
// Created: 9:24 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using RedScooby.Infrastructure.Composition;
using RedScooby.Logging;
using RedScooby.Views.Components;

namespace RedScooby.Views.Setup
{
    public class PreStartNotesViewBase : PageView<IViewModel> { }

    public sealed partial class PreStartNotesView
    {
        private LaunchActivatedEventArgs launchActivatedEventArgs;

        public PreStartNotesView()
        {
            InitializeComponent();
            VisualStateManager.GoToState(this, EmptyViewState.Name, false);
            Loaded += (sender, args) => { VisualStateManager.GoToState(this, LoadedViewState.Name, true); };
        }

        public void CarryForwardLaunchEventArgs(LaunchActivatedEventArgs args)
        {
            launchActivatedEventArgs = args;
        }

        private void StartAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Log.Trace("Start app button clicked");
            if (ScrollTextViewer.VerticalOffset <
                ScrollTextViewer.ScrollableHeight - (7.5*(ScrollTextViewer.ViewportHeight/100)))
            {
                Log.Trace("Scroll down initiated");
                ScrollTextViewer.ChangeView(null, (ScrollTextViewer.VerticalOffset + ScrollTextViewer.ActualHeight)/1.15,
                    null);
            }
            else
            {
                Log.Trace("Loading main page");
                ShellHelper.LoadMainPageIfRequired(launchActivatedEventArgs);
            }
        }
    }
}
