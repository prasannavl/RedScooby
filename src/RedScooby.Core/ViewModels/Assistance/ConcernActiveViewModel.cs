// Author: Prasanna V. Loganathar
// Created: 9:16 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using RedScooby.Actions;
using RedScooby.Common.Resources;
using RedScooby.Components;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.Infrastructure.Location;

namespace RedScooby.ViewModels.Assistance
{
    public sealed class ConcernActiveViewModel : ViewModelBase
    {
        private static readonly string[] _infoStrings = new[]
        {
            CommonStrings.ConcernModeInfo,
            "Quick Tip: Tap on the time on the right to quickly halve the timer.",
            "Quick Tip: Tap on the timer icon on the left to refill the timer right away.",
        };

        private readonly ConcernManager concernManager;
        private Address address;
        private string timeLeftForNextPromptString;
        private double timerProgressValue;
        private ConcernNotificationOptionDetail notificationPreference;
        private IDisposable feedbackTimer;
        private double nextFeedbackPeriodSeconds;
        private ObservableCollection<ConcernNotificationOptionDetail> notificationOptions;
        private int currentInfoIndex = 1;
        private string infoText = _infoStrings[0];

        public ConcernActiveViewModel(ConcernManager concernManager, DistressManager distressManager,
            FeedbackManager feedbackManager,
            LocationManager locationManager)
        {
            this.concernManager = concernManager;

            ResetFeedbackTimerDelegate();

            EndConcernAsyncCommand =
                CommandFactory.CreateAsync(async () => { await concernManager.RequestEndConcern(); });
            ForwardMessageToView(ViewActions.Concern.TurnOff, DisposeFeedbackTimer);

            SubscribeTo(ViewModelActions.Concern.FeedbackTimerRequery, ResetFeedbackTimerDelegate);

            SetupLocationRoutines(locationManager);

            SetupNotificationPreferences();
            SetupTextScroller();

            NotificationPreference =
                NotificationOptions.First(x => x.OptionTitle == concernManager.CurrentNotificationPreference);

            HalveFeedbackTimeCommand = CommandFactory.Create(SlashFeedbackTimeByHalf);
            RequestImmediateFeedbackAsyncCommand =
                CommandFactory.CreateAsync(async () =>
                {
                    if (distressManager.GetCurrentState() == DistressState.Countdown)
                        await distressManager.RequestCancelDistressCountdown();
                    else
                        await concernManager.RequestFeedbackAsync(true);
                });
        }

        public AsyncRelayCommand RequestImmediateFeedbackAsyncCommand { get; private set; }
        public RelayCommand HalveFeedbackTimeCommand { get; private set; }
        public AsyncRelayCommand EndConcernAsyncCommand { get; private set; }

        public ObservableCollection<ConcernNotificationOptionDetail> NotificationOptions
        {
            get { return notificationOptions; }
            set { SetAndNotifyIfChanged(ref notificationOptions, value); }
        }

        public ConcernNotificationOptionDetail NotificationPreference
        {
            get { return notificationPreference; }
            set
            {
                SetAndNotifyIfChanged(ref notificationPreference, value,
                    () => { concernManager.SetNotificationPreference(value.OptionTitle); });
            }
        }

        public string InfoText
        {
            get { return infoText; }
            set { SetAndNotifyIfChanged(ref infoText, value); }
        }

        public Address Address
        {
            get { return address; }
            private set { SetAndNotifyIfChanged(ref address, value); }
        }

        public string TimeLeftForNextPromptString
        {
            get { return timeLeftForNextPromptString; }
            private set { SetAndNotifyIfChanged(ref timeLeftForNextPromptString, value); }
        }

        public double TimerProgressValue
        {
            get { return timerProgressValue; }
            private set { SetAndNotifyIfChanged(ref timerProgressValue, value); }
        }

        private void SetupTextScroller()
        {
            AddDisposable(
                Observable.Interval(TimeSpan.FromMilliseconds(4500)).Subscribe(i => { ToggleConcernInfoText(); }));
        }

        private void ToggleConcernInfoText()
        {
            if (currentInfoIndex >= _infoStrings.Length)
                currentInfoIndex = 0;
            InfoText = _infoStrings[currentInfoIndex++];
        }

        private void SetupLocationRoutines(LocationManager locationManager)
        {
            var lastKnownAddress = locationManager.LastKnownAddress;

            Address = (lastKnownAddress == Address.UnknownAddress ||
                       lastKnownAddress.Position.Timestamp <
                       DateTimeOffset.Now.Subtract(LocationManager.DefaultRecentAddressTimeSpan))
                ? new Address(GeoPosition.ZeroPosition, CommonStrings.DeterminingLocation)
                : lastKnownAddress;

            AddDisposable(locationManager.PositionChanges.Subscribe(x => locationManager.UpdateAddressAsync()));
            AddDisposable(locationManager.AddressChanges.Subscribe(x => Address = x));

            locationManager.EnsureRecentAddressAsync();
        }

        private void SetupNotificationPreferences()
        {
            // TODO: Move strings to resources.

            notificationOptions = new ObservableCollection<ConcernNotificationOptionDetail>
            {
                new ConcernNotificationOptionDetail
                {
                    OptionDescription = "Smart notifications (Recommended)",
                    OptionTitle = ConcernNotificationPreference.Auto
                },
                new ConcernNotificationOptionDetail
                {
                    OptionDescription = "I don't want anyone to know",
                    OptionTitle = ConcernNotificationPreference.Nobody
                },
                new ConcernNotificationOptionDetail
                {
                    OptionDescription = "My inner circles only",
                    OptionTitle = ConcernNotificationPreference.Circles
                },
                new ConcernNotificationOptionDetail
                {
                    OptionDescription = "Nearest professionals only",
                    OptionTitle = ConcernNotificationPreference.Professionals
                },
                new ConcernNotificationOptionDetail
                {
                    OptionDescription = "My inner circles and nearest professionals",
                    OptionTitle = ConcernNotificationPreference.CirclesAndProfessionals
                },
            };
        }

        private void DisposeFeedbackTimer()
        {
            if (feedbackTimer != null)
            {
                feedbackTimer.Dispose();
                feedbackTimer = null;
            }
        }

        private void SlashFeedbackTimeByHalf()
        {
            var now = DateTimeOffset.Now;
            var next = concernManager.NextFeedbackTime;
            if (next > now)
            {
                var remaining = next - now;
                if (remaining.TotalSeconds > 2)
                {
                    concernManager.SetFeedbackTimeInterval(TimeSpan.FromMilliseconds(remaining.TotalMilliseconds/2));
                }
            }
        }

        private void ResetFeedbackTimerDelegate()
        {
            DisposeFeedbackTimer();
            nextFeedbackPeriodSeconds = Math.Round((concernManager.NextFeedbackTime - DateTimeOffset.Now).TotalSeconds);
            if (nextFeedbackPeriodSeconds <= 0) nextFeedbackPeriodSeconds = 0;
            else
            {
                feedbackTimer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)).Subscribe(t =>
                {
                    if (t < nextFeedbackPeriodSeconds)
                    {
                        var secondsLeft = nextFeedbackPeriodSeconds - t;
                        TimeLeftForNextPromptString = TimeSpan.FromSeconds(secondsLeft).ToString(@"mm\:ss");
                        TimerProgressValue = (secondsLeft/nextFeedbackPeriodSeconds)*100;
                    }
                    else
                    {
                        TimeLeftForNextPromptString = "00:00";
                        TimerProgressValue = 0;
                        DisposeFeedbackTimer();
                    }
                });
            }
        }
    }

    public class ConcernNotificationOptionDetail
    {
        public ConcernNotificationPreference OptionTitle { get; set; }
        public string OptionDescription { get; set; }
    }
}
