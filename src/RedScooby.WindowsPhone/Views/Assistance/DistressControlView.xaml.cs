// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using RedScooby.Actions;
using RedScooby.Components;
using RedScooby.Helpers;
using RedScooby.Logging;
using RedScooby.ViewModels.Assistance;
using RedScooby.Views.Components;

namespace RedScooby.Views.Assistance
{
    public class DistressControlViewBase : UserControlView<DistressControlViewModel> { }

    public partial class DistressControlView
    {
        private readonly HybridInterlockedMonitor monitor = new HybridInterlockedMonitor();
        private Brush backgroundBrush;
        private DistressState currentViewState;

        public DistressControlView()
        {
            InitializeComponent();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            backgroundBrush = DistressOnButton.Background;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            SubscribeTo(ViewActions.Distress.ActivateCountdown, async () => await ActivateCountdownViewAsync());
            SubscribeTo(ViewActions.Distress.DeactivateCountdown, async () => await DeactivateCountdownViewAsync());

            SubscribeTo(ViewActions.AssistView.TapAndHoldFocus, () =>
            {
                Log.Trace("DistressControlView: Tap and hold mode");
                DistressOnButton.Background = new SolidColorBrush(Color.FromArgb(255, 74, 191, 53));
            });

            SubscribeTo(ViewActions.AssistView.TapAndHoldRelease, () =>
            {
                Log.Trace("DistressControlView: Tap and hold released");
                DistressOnButton.Background = backgroundBrush;
                Model.ActivateCountdownForTapAndHoldCommand.Execute(null);
            });

            SetCurrentState()
                .ContinueWithErrorHandling();
        }

        private async Task SetCurrentState()
        {
            try
            {
                await monitor.EnterAsync();
                var pastState = currentViewState;
                var newState = Model.CurrentDistressState;
                if (newState == pastState) return;

                await
                    (newState == DistressState.Off
                        ? DeactivateCountdownViewAsync(true)
                        : ActivateCountdownViewAsync(true));
            }
            finally
            {
                monitor.Exit();
            }
        }

        private async Task ActivateCountdownViewAsync(bool lockTaken = false)
        {
            try
            {
                if (!lockTaken) await monitor.EnterAsync();
                currentViewState = DistressState.Countdown;
                DistressCountdownViewHolder.Activate(false);
                await CountdownStoryboard.RunAsync();
            }
            finally
            {
                if (!lockTaken) monitor.Exit();
            }
        }

        private async Task DeactivateCountdownViewAsync(bool lockTaken = false)
        {
            try
            {
                if (!lockTaken) await monitor.EnterAsync();
                currentViewState = DistressState.Off;
                DistressOnButton.Background = backgroundBrush;
                await CountdownStoryboard.RunReverseAsync();
                DistressCountdownViewHolder.Deactivate(false);
            }
            finally
            {
                if (!lockTaken) monitor.Exit();
            }
        }
    }
}
