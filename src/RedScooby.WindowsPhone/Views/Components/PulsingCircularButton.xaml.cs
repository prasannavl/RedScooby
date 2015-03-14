// Author: Prasanna V. Loganathar
// Created: 9:20 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using LiquidState;
using LiquidState.Configuration;
using LiquidState.Machines;
using RedScooby.Common;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Framework;
using RedScooby.Utilities;

namespace RedScooby.Views.Components
{
    public sealed partial class PulsingCircularButton
    {
        private readonly object pulseSync = new object();
        private IAwaitableStateMachine<ActivityState, ActivityState> stateMachine;
        private volatile bool isPulsing;
        private TaskCompletionSource<bool> currentPulseCompletion;

        public PulsingCircularButton()
        {
            InitializeComponent();
            Loaded +=
                (sender, args) =>
                {
                    stateMachine = StateMachine.Create(ActivityState.Inactive, GetConfig(this, Background));
                };
        }

        public ActivityState GetState()
        {
            return stateMachine == null ? ActivityState.Inactive : stateMachine.CurrentState;
        }

        public Task SetState(ActivityState targetState)
        {
            return stateMachine == null ? TaskCache.Completed : stateMachine.FireAsync(targetState);
        }

        private static AwaitableStateMachineConfiguration<ActivityState, ActivityState> GetConfig(
            PulsingCircularButton button, Brush brush)
        {
            var config = StateMachine.CreateAwaitableConfiguration<ActivityState, ActivityState>();

            config.ForState(ActivityState.Active)
                .OnEntry(async () => { await button.ScaleStoryboard.RunAsync(); })
                .Ignore(ActivityState.Active)
                .Permit(ActivityState.Inactive, ActivityState.Inactive);

            config.ForState(ActivityState.InProgress)
                .Ignore(ActivityState.InProgress)
                .OnEntry(() =>
                {
                    button.isPulsing = true;
                    button.PulseAsync();
                })
                .OnExit(async () =>
                {
                    var task = button.PulseAsync();
                    button.isPulsing = false;
                    await task;
                })
                .Permit(ActivityState.Inactive, ActivityState.Inactive)
                .Permit(ActivityState.Active, ActivityState.Active);

            config.ForState(ActivityState.Inactive)
                .OnEntry(() => { button.Background = brush; })
                .Ignore(ActivityState.Inactive)
                .Permit(ActivityState.InProgress, ActivityState.InProgress)
                .Permit(ActivityState.Active, ActivityState.Active);

            return config;
        }

        private Task PulseAsync()
        {
            lock (pulseSync)
            {
                if (currentPulseCompletion != null)
                    return currentPulseCompletion.Task;

                currentPulseCompletion = new TaskCompletionSource<bool>();
            }

            SchedulePulse();
            return currentPulseCompletion.Task;
        }

        private void SchedulePulse()
        {
            PulseStoryboard.RunAsync().ContinueWith(t =>
            {
                if (isPulsing)
                    SchedulePulse();
                else
                {
                    lock (pulseSync)
                    {
                        currentPulseCompletion.TrySetResult(true);
                        currentPulseCompletion = null;
                    }
                }
            }, CancellationToken.None, TaskContinuationOptions.DenyChildAttach, DispatchHelper.Current.Scheduler);
        }
    }
}
