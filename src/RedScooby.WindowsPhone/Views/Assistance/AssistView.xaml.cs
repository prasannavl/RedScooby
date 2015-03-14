// Author: Prasanna V. Loganathar
// Created: 9:17 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using RedScooby.Actions;
using RedScooby.Components;
using RedScooby.Helpers;
using RedScooby.Logging;
using RedScooby.ViewModels.Assistance;
using RedScooby.Views.Components;

namespace RedScooby.Views.Assistance
{
    public class AssistViewBase : UserControlView<AssistanceViewModel> { }

    public partial class AssistView
    {
        private const string OpacityPropertyName = "Opacity";
        private readonly HybridInterlockedMonitor monitor = new HybridInterlockedMonitor();
        private int isButtonHeld;
        private DistressState currentDistressViewState;
        private bool isFeedbackCurrentViewStateVisible;

        public AssistView()
        {
            InitializeComponent();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            SubscribeTo(ViewActions.PinFeedback.Show, () => { SetCurrentFeedbackState().ContinueWithErrorHandling(); });
            SubscribeTo(ViewActions.PinFeedback.Hide, () => { SetCurrentFeedbackState().ContinueWithErrorHandling(); });
            SubscribeTo(ViewActions.Distress.TurnOn, () => { SetCurrentDistressState().ContinueWithErrorHandling(); });
            SubscribeTo(ViewActions.Distress.TurnOff, () => { SetCurrentDistressState().ContinueWithErrorHandling(); });

            SetCurrentState()
                .ContinueWithErrorHandling();
        }

        private async Task SetCurrentState()
        {
            await monitor.EnterAsync();
            try
            {
                await SetCurrentDistressState(true);
                await SetCurrentFeedbackState(true);
            }
            finally
            {
                monitor.Exit();
            }
        }

        private async Task SetCurrentDistressState(bool lockTaken = false)
        {
            try
            {
                if (!lockTaken) await monitor.EnterAsync();
                var pastState = currentDistressViewState;
                var newState = Model.CurrentDistressState;
                currentDistressViewState = newState;

                if (newState == pastState) return;

                await (newState == DistressState.On
                    ? ActivateDistressView(true)
                    : DeactivateDistressView(true));
            }
            finally
            {
                if (!lockTaken) monitor.Exit();
            }
        }

        private async Task SetCurrentFeedbackState(bool lockTaken = false)
        {
            try
            {
                if (!lockTaken) await monitor.EnterAsync();
                var pastState = isFeedbackCurrentViewStateVisible;
                var newState = Model.PinFeedbackViewStateVisible;
                isFeedbackCurrentViewStateVisible = newState;

                if (newState == pastState) return;
                await (newState
                    ? ActivatePinFeedbackView(true)
                    : DeactivatePinFeedbackView(true));
            }
            finally
            {
                if (!lockTaken) monitor.Exit();
            }
        }

        private async Task FocusTapAndHold()
        {
            Log.Trace("AssistView: Entering tap and hold");
            ViewMessenger.Send(ViewActions.RootView.Lock);

            var tasks = new List<Task>(3);

            foreach (
                DependencyObject obj in
                    new object[] {ConcernViewRegion, SirenButton, FlashlightButton})
            {
                var storyboard = new Storyboard();
                var anim = new DoubleAnimation {Duration = TimeSpan.FromMilliseconds(200), From = 1, To = 0.3};
                storyboard.Children.Add(anim);
                Storyboard.SetTarget(anim, obj);
                Storyboard.SetTargetProperty(anim, OpacityPropertyName);
                tasks.Add(storyboard.RunAsync());
            }

            await Task.WhenAll(tasks);

            Log.Trace("AssistView: Entered tap and hold");
        }

        private async Task ReleaseTapAndHold()
        {
            Log.Trace("AssistView: Releasing tap and hold");
            var tasks = new List<Task>(3);


            foreach (
                DependencyObject obj in
                    new object[] {ConcernViewRegion, SirenButton, FlashlightButton})
            {
                var storyboard = new Storyboard();
                var anim = new DoubleAnimation {Duration = TimeSpan.FromMilliseconds(200), From = 0.3, To = 1};
                storyboard.Children.Add(anim);
                Storyboard.SetTarget(anim, obj);
                Storyboard.SetTargetProperty(anim, OpacityPropertyName);
                tasks.Add(storyboard.RunAsync());
            }

            await Task.WhenAll(tasks);
            Log.Trace("AssistView: Released tap and hold");

            ViewMessenger.Send(ViewActions.RootView.Unlock);
        }

        private async Task ActivateDistressView(bool lockTaken = false)
        {
            try
            {
                if (!lockTaken) await monitor.EnterAsync();
                ViewMessenger.Send(ViewActions.RootView.FocusAssist);
                DistressActiveViewHolder.Activate();
                await DistressStoryboard.RunAsync();
            }
            finally
            {
                if (!lockTaken) monitor.Exit();
            }
        }

        private async Task ActivatePinFeedbackView(bool lockTaken = false)
        {
            try
            {
                if (!lockTaken) await monitor.EnterAsync();
                ViewMessenger.Send(ViewActions.RootView.FocusAssist);
                PinFeedbackViewHolder.Activate();
                await PinFeedbackStoryboard.RunAsync();
            }
            finally
            {
                if (!lockTaken) monitor.Exit();
            }
        }

        private async Task DeactivatePinFeedbackView(bool lockTaken = false)
        {
            try
            {
                if (!lockTaken) await monitor.EnterAsync();
                ViewMessenger.Send(ViewActions.RootView.FocusAssist);
                await PinFeedbackStoryboard.RunReverseAsync();
                PinFeedbackViewHolder.Deactivate();
            }
            finally
            {
                if (!lockTaken) monitor.Exit();
            }
        }

        private async Task DeactivateDistressView(bool lockTaken = false)
        {
            try
            {
                if (!lockTaken) await monitor.EnterAsync();
                ViewMessenger.Send(ViewActions.RootView.FocusAssist);
                await DistressStoryboard.RunReverseAsync();
                DistressActiveViewHolder.Deactivate();
            }
            finally
            {
                if (!lockTaken) monitor.Exit();
            }
        }

        private void TapAndHoldPointerReleased()
        {
            if (Interlocked.CompareExchange(ref isButtonHeld, 2, 1) == 1)
            {
                var _ = ReleaseTapAndHold().ContinueWith(t =>
                {
                    ViewMessenger.Send(ViewActions.AssistView.TapAndHoldRelease);
                    Interlocked.Exchange(ref isButtonHeld, 0);
                });
            }
        }

        private void DistressViewRegion_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (Interlocked.CompareExchange(ref isButtonHeld, 1, 0) == 0)
            {
                var _ =
                    FocusTapAndHold()
                        .ContinueWith(t => { ViewMessenger.Send(ViewActions.AssistView.TapAndHoldFocus); });
            }
        }

        private void DistressViewRegion_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            TapAndHoldPointerReleased();
        }
    }
}
