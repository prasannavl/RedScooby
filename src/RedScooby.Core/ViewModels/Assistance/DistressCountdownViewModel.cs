// Author: Prasanna V. Loganathar
// Created: 9:16 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Linq;
using RedScooby.Actions;
using RedScooby.Components;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;

namespace RedScooby.ViewModels.Assistance
{
    public class DistressCountdownViewModel : ViewModelBase
    {
        private readonly DistressManager distressManager;
        private double countdown;
        private double countdownProgressValue;
        private IDisposable distressCountdownProgressSubscription;

        public DistressCountdownViewModel(DistressManager distressManager)
        {
            this.distressManager = distressManager;
            ForwardMessageToView(ViewActions.Distress.DeactivateCountdown, DisposeCountdownTimer);
            CancelDistressAsyncCommand = CommandFactory.CreateAsync(distressManager.RequestCancelDistressCountdown);

            StartCountdown();
            CountdownProgressValue = 100;
        }

        public AsyncRelayCommand CancelDistressAsyncCommand { get; private set; }

        public double Countdown
        {
            get { return countdown; }
            set { SetAndNotifyIfChanged(ref countdown, value); }
        }

        public double CountdownProgressValue
        {
            get { return countdownProgressValue; }
            set { SetAndNotifyIfChanged(ref countdownProgressValue, value); }
        }

        private void StartCountdown()
        {
            DisposeCountdownTimer();
            var expirySeconds = Math.Round((distressManager.NextCountdownExpiryTime - DateTimeOffset.Now).TotalSeconds);
            distressCountdownProgressSubscription = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                .Subscribe(t =>
                {
                    if (t < expirySeconds)
                    {
                        Countdown = expirySeconds - t;
                        CountdownProgressValue = (Countdown/DistressManager.CountdownInterval.Seconds)*100.0;
                    }
                    else
                    {
                        Countdown = 0;
                        CountdownProgressValue = 0;
                        DisposeCountdownTimer();
                    }
                });
        }

        private void DisposeCountdownTimer()
        {
            if (distressCountdownProgressSubscription != null)
            {
                distressCountdownProgressSubscription.Dispose();
                distressCountdownProgressSubscription = null;
            }
        }
    }
}
