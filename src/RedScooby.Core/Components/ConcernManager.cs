// Author: Prasanna V. Loganathar
// Created: 7:14 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RedScooby.Actions;
using RedScooby.Api;
using RedScooby.Data.Tables;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Composition;
using RedScooby.Logging;
using RedScooby.Models;
using RedScooby.Utilities;

// ReSharper disable InconsistentlySynchronizedField

namespace RedScooby.Components
{
    public class ConcernManager : IDisposable
    {
        public static TimeSpan FeedbackCautiousTimeInterval = TimeSpan.FromSeconds(45);
        public static TimeSpan FeedbackDefaultTimeInterval = TimeSpan.FromMinutes(15);
        public static TimeSpan FeedbackDemoModeTimeInterval = TimeSpan.FromSeconds(15);
        public static TimeSpan WaitForPeriodicFeedbackTimeInterval = TimeSpan.FromSeconds(30);
        public static TimeSpan WaitForTurnOffFeedbackTimeInterval = TimeSpan.FromSeconds(10);
        // Fallback time-intervals. Reporting happens on these time intervals, only if no other
        // event, such as significant location changes triggers it in the given interval.
        public static TimeSpan PeriodicReportDefaultTimeInterval = TimeSpan.FromSeconds(60);
        public static TimeSpan PeriodicReportTimeInterval = PeriodicReportDefaultTimeInterval;
        private static readonly ILogger Log = Logging.Log.ForContext<ConcernManager>();
        private readonly DistressManager distressManager;
        private readonly FeedbackManager feedbackManager;
        private readonly HotlineManager hotlineManager;
        private readonly AppModel model;
        private readonly object syncRoot = new object();
        private ConcernState currentState = ConcernState.Off;
        private TimeSpan feedbackTimeInterval = FeedbackDefaultTimeInterval;
        private IDisposable feedbackTimer;
        private DateTimeOffset lastTimerSetupTime;

        public ConcernManager(FeedbackManager feedbackManager, DistressManager distressManager,
            HotlineManager hotlineManager, AppModel model)
        {
            this.feedbackManager = feedbackManager;
            this.distressManager = distressManager;
            this.hotlineManager = hotlineManager;
            this.model = model;
        }

        public ConcernNotificationPreference CurrentNotificationPreference { get; private set; }
        public DateTimeOffset NextFeedbackTime { get; private set; }

        public void Dispose()
        {
            DisposeFeedbackTimer();
        }

        public void SaveState()
        {
            var state = GetCurrentState();
            if (state == ConcernState.On)
            {
                var settings = model.LocalSettings;
                settings.LastConcernState = new ConcernStatePersistence
                {
                    State = state,
                    FeedbackTimeInterval = feedbackTimeInterval,
                    NextFeedbackTime = NextFeedbackTime,
                };
            }
            else
            {
                model.LocalSettings.LastConcernState = null;
            }
        }

        public void RestoreState()
        {
            var state = model.LocalSettings.LastConcernState;
            if (state == null) return;

            var current = GetCurrentState();

            NextFeedbackTime = state.NextFeedbackTime;
            feedbackTimeInterval = state.FeedbackTimeInterval;
            var timeLeft = NextFeedbackTime - DateTimeOffset.Now;
            if (timeLeft.TotalSeconds > 0)
            {
                if (current == ConcernState.Off)
                {
                    SetupFeedbackTimer(timeLeft);
                    StartConcern(false, true);
                }
                else
                {
                    ViewModel.Messenger.Send(ViewModelActions.Concern.FeedbackTimerRequery);
                }
            }
            else if (timeLeft.TotalSeconds + 30 > 0)
            {
                DistressManager.HintDistressPossibility(model, true);
            }
            else
            {
                DistressManager.HintDistressPossibility(model);
            }
        }

        public void EndConcern()
        {
            Log.Trace("Ending concern");

            lock (syncRoot)
            {
                if (currentState != ConcernState.Off)
                {
                    currentState = ConcernState.Off;

                    DisposeFeedbackTimer();

                    ViewModel.Messenger.Send(ViewActions.Concern.TurnOff);

                    if (!hotlineManager.IsDistressActive)
                    {
                        RedScoobyApi.Client.Concern.Deactivate(DateTimeOffset.Now)
                            .ContinueWithErrorHandling();

                        hotlineManager.ResetPeriodicReporting();
                    }

                    Log.Trace("Concern ended");
                }
            }
        }

        public ConcernState GetCurrentState()
        {
            lock (syncRoot)
            {
                return currentState;
            }
        }

        public void RefillFeedbackTimerIfPossible()
        {
            lock (syncRoot)
            {
                if (currentState == ConcernState.On)
                {
                    SetFeedbackTimeInterval(DeduceNextFeedbackInterval());
                }
            }
        }

        public Task RequestEndConcern()
        {
            Log.Trace("End concern request");

            if (currentState == ConcernState.On)
            {
                DisposeFeedbackTimer();

                if (distressManager.GetCurrentState() == DistressState.Countdown)
                {
                    return distressManager.RequestCancelDistressCountdown();
                }

                return feedbackManager.RequestFeedbackAsync(FeedbackManager.FeedbackContext.ConcernDeactivation,
                    WaitForTurnOffFeedbackTimeInterval)
                    .ContinueWith(t =>
                    {
                        t.HandleError(true);

                        var res = t.Result;
                        Log.Trace("Feedback result: {0}", res);

                        switch (res)
                        {
                            case FeedbackManager.FeedbackResult.Success:
                            {
                                EndConcern();
                                break;
                            }
                            case FeedbackManager.FeedbackResult.FailCovertly:
                            {
                                if (!distressManager.IsCovertDistressOn)
                                    distressManager.StartCovertDistress();
                                EndConcern();
                                break;
                            }
                            case FeedbackManager.FeedbackResult.Suppressed:
                            {
                                Log.Info("Deactivation feedback suppressed. This should've never happened");
                                break;
                            }
                            case FeedbackManager.FeedbackResult.Failure:
                            case FeedbackManager.FeedbackResult.NoFeedback:
                            default:
                            {
                                distressManager.StartDistressCountdown();
                                break;
                            }
                        }
                    });
            }
            return TaskCache.Completed;
        }

        public async Task RequestFeedbackAsync(bool manualRequest = false)
        {
            Log.Trace("Requesting feedback");
            var res = await feedbackManager.RequestFeedbackAsync(FeedbackManager.FeedbackContext.ConcernPeriodic,
                WaitForPeriodicFeedbackTimeInterval);

            Log.Trace("Feedback result: {0}", res);

            switch (res)
            {
                case FeedbackManager.FeedbackResult.Success:
                {
                    lock (syncRoot)
                    {
                        SetFeedbackTimeIntervalUnsafe(manualRequest
                            ? FeedbackDefaultTimeInterval
                            : DeduceNextFeedbackInterval());
                    }
                    break;
                }
                case FeedbackManager.FeedbackResult.FailCovertly:
                {
                    if (!distressManager.IsCovertDistressOn)
                        distressManager.StartCovertDistress();
                    lock (syncRoot)
                    {
                        SetFeedbackTimeIntervalUnsafe(DeduceNextFeedbackInterval());
                    }
                    break;
                }
                case FeedbackManager.FeedbackResult.Suppressed:
                {
                    Log.Info("Periodic feedback supressed");
                    break;
                }
                case FeedbackManager.FeedbackResult.Failure:
                case FeedbackManager.FeedbackResult.NoFeedback:
                default:
                {
                    distressManager.StartDistressCountdown();
                    break;
                }
            }
        }

        public void SetFeedbackTimeInterval(TimeSpan timeSpan)
        {
            lock (syncRoot)
            {
                SetFeedbackTimeIntervalUnsafe(timeSpan);
            }
        }

        public void SetNotificationPreference(ConcernNotificationPreference preference)
        {
            if (CurrentNotificationPreference != preference)
            {
                CurrentNotificationPreference = preference;
                if (GetCurrentState() == ConcernState.On)
                    hotlineManager.ReportNowAsync()
                        .ContinueWithErrorHandling();
            }
        }

        public void StartConcern(bool useCautiousPreset = false, bool manualTimer = false)
        {
            Log.Trace("Concern start request");
            lock (syncRoot)
            {
                if (currentState != ConcernState.On)
                {
                    Log.Trace("Concern starting");

                    currentState = ConcernState.On;
                    ViewModel.Messenger.Send(ViewActions.Concern.TurnOn);

                    if (!hotlineManager.IsDistressActive)
                    {
                        RedScoobyApi.Client.Concern.Activate(DateTimeOffset.Now)
                            .ContinueWithErrorHandling();

                        hotlineManager.SwitchReporting(HotlineManager.TrueReportingMode.Concern,
                            PeriodicReportTimeInterval, true);
                    }

                    if (!manualTimer)
                    {
                        if (useCautiousPreset)
                        {
                            if (feedbackTimeInterval > FeedbackCautiousTimeInterval)
                                feedbackTimeInterval = FeedbackCautiousTimeInterval;
                        }
                        else
                        {
                            feedbackTimeInterval = DeduceNextFeedbackInterval();
                        }

                        SetFeedbackTimeIntervalUnsafe(feedbackTimeInterval);
                    }

                    Log.Trace("Concern started");
                }
            }
        }

        private TimeSpan DeduceNextFeedbackInterval()
        {
            var ts = feedbackTimeInterval;
            if (ts < FeedbackDefaultTimeInterval)
            {
                if (lastTimerSetupTime > DateTimeOffset.Now.Subtract(TimeSpan.FromSeconds(9)))
                {
                    return FeedbackDefaultTimeInterval;
                }

                var flag = false;
                while (ts <= FeedbackCautiousTimeInterval)
                {
                    ts = TimeSpan.FromSeconds(ts.TotalSeconds*2);
                    flag = true;
                }
                if (flag) return ts;

                if (ts < FeedbackDefaultTimeInterval) ts = TimeSpan.FromSeconds(ts.TotalSeconds*2);
            }

            if (ts > FeedbackDefaultTimeInterval) ts = FeedbackDefaultTimeInterval;

            return ts;
        }

        private void DisposeFeedbackTimer()
        {
            if (feedbackTimer != null)
            {
                Log.Trace("Dispose feedback timer");
                feedbackTimer.Dispose();
                feedbackTimer = null;
            }
        }

        private void SetFeedbackTimeIntervalUnsafe(TimeSpan timeSpan)
        {
            feedbackTimeInterval = timeSpan;
            if (currentState == ConcernState.On)
            {
                SetupFeedbackTimer(timeSpan);
                lastTimerSetupTime = DateTimeOffset.Now;
                hotlineManager.ReportNowAsync()
                    .ContinueWithErrorHandling();
            }
        }

        private void SetupFeedbackTimer(TimeSpan timeInterval)
        {
            Log.Trace("Reset feedback timer: {0}", timeInterval.ToString());

            DisposeFeedbackTimer();
            NextFeedbackTime = DateTimeOffset.Now.Add(timeInterval);
            Log.Trace("Next feedback on: {0}", NextFeedbackTime.ToString());

            feedbackTimer = Observable.Timer(timeInterval).Subscribe(async i => { await RequestFeedbackAsync(); });

            ViewModel.Messenger.Send(ViewModelActions.Concern.FeedbackTimerRequery);
        }
    }

    public enum ConcernNotificationPreference
    {
        Auto,
        Nobody,
        Professionals,
        Circles,
        CirclesAndProfessionals,
        Custom
    }

    public enum ConcernState
    {
        Off,
        On
    }
}
