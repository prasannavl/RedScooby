// Author: Prasanna V. Loganathar
// Created: 7:14 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RedScooby.Actions;
using RedScooby.Api;
using RedScooby.Common;
using RedScooby.Data.Tables;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Composition;
using RedScooby.Logging;
using RedScooby.Models;
using RedScooby.Utilities;

// ReSharper disable InconsistentlySynchronizedField

namespace RedScooby.Components
{
    public class DistressManager : IDisposable
    {
        public static TimeSpan CountdownInterval = TimeSpan.FromSeconds(4);
        public static TimeSpan WaitForCancellationFeedbackInterval = TimeSpan.FromSeconds(10);
        // Fallback time-intervals. Reporting happens on these time intervals, only if no other
        // event, such as significant location changes triggers it in the given interval. 

        public static TimeSpan PeriodicReportDefaultTimeInterval = TimeSpan.FromSeconds(30);
        public static TimeSpan PeriodicReportDistressTimeInterval = PeriodicReportDefaultTimeInterval;
        public static TimeSpan PeriodicReportCovertDistressTimeInterval = PeriodicReportDefaultTimeInterval;
        private static readonly ILogger Log = Logging.Log.ForContext<DistressManager>();
        private readonly AppModel model;
        private readonly FeedbackManager feedbackManager;
        private readonly HotlineManager hotlineManager;
        private readonly Lazy<ConcernManager> concernManager;
        private readonly object syncRoot = new object();
        private DistressState currentState;
        private IDisposable countdownTimerSubscription;
        private IList<CategoryActivityState> categoryStates;

        public DistressManager(AppModel model, FeedbackManager feedbackManager, HotlineManager hotlineManager,
            Lazy<ConcernManager> concernManager)
        {
            this.model = model;
            this.feedbackManager = feedbackManager;
            this.hotlineManager = hotlineManager;
            this.concernManager = concernManager;

            ResetCategoryStates();
        }

        public DistressCategory DesiredCategory { get; private set; }
        public DateTimeOffset NextCountdownExpiryTime { get; private set; }
        public bool IsCovertDistressOn { get; private set; }
        public DistressNotificationPreference CurrentNotificationPreference { get; private set; }

        public void Dispose()
        {
            DisposeCountdownTimer();
        }

        public static void HintDistressPossibility(AppModel model, bool tryCountdown = false)
        {
            if (model.LocalSettings.LastDistressState == null)
            {
                model.LocalSettings.LastDistressState =
                    new DistressPersistenceState
                    {
                        State = tryCountdown ? DistressState.Countdown : DistressState.On,
                    };
            }
            else
            {
                if (model.LocalSettings.LastDistressState.State == DistressState.Off)
                    model.LocalSettings.LastDistressState.State = tryCountdown
                        ? DistressState.Countdown
                        : DistressState.On;
            }
        }

        public CategoryActivityState GetCategoryState(DistressCategory category)
        {
            return categoryStates[(int) category];
        }

        public async Task ToggleCategory(DistressCategory category)
        {
            DesiredCategory = category;
            var cState = GetCategoryState(category);
            if (cState.State == ActivityState.Inactive)
            {
                DesiredCategory |= category;

                cState.State = ActivityState.InProgress;
                var current = DateTimeOffset.Now;
                cState.DateTimeOffset = current;
                ViewModel.Messenger.Send(ViewModelActions.Distress.CategoryRequery);

                if (GetCurrentState() == DistressState.On)
                {
                    var _ = hotlineManager.ReportNowAsync()
                        .ContinueWithErrorHandling();
                }
                // TODO: Change to real notification mechanism
                await Task.Delay(3000);

                // Requery, and make sure its unchanged.
                cState = GetCategoryState(category);
                if (cState.DateTimeOffset == current)
                {
                    cState.State = ActivityState.Active;
                    ViewModel.Messenger.Send(ViewModelActions.Distress.CategoryRequery);
                }
            }
            else
            {
                DesiredCategory &= ~category;
                cState.State = ActivityState.Inactive;
                cState.DateTimeOffset = DateTimeOffset.Now;
                ViewModel.Messenger.Send(ViewModelActions.Distress.CategoryRequery);

                if (GetCurrentState() == DistressState.On)
                {
                    var _ = hotlineManager.ReportNowAsync()
                        .ContinueWithErrorHandling();
                }
            }
        }

        public void CancelDistressCountdown()
        {
            Log.Trace("Cancel countdown request");

            lock (syncRoot)
            {
                CancelDistressCountdownUnsafe();
            }
        }

        public void EndDistress()
        {
            Log.Trace("End distress request");

            lock (syncRoot)
            {
                EndDistressUnsafe();
            }
        }

        public DistressState GetCurrentState()
        {
            lock (syncRoot)
            {
                return currentState;
            }
        }

        public Task RequestCancelDistressCountdown()
        {
            Log.Trace("Request for countdown cancellation");

            lock (syncRoot)
            {
                return RequestCancelDistressCountdownUnsafe();
            }
        }

        public Task RequestEndDistress()
        {
            Log.Trace("Request for stop");

            lock (syncRoot)
            {
                return RequestEndDistressUnsafe();
            }
        }

        public void SetNotificationPreference(DistressNotificationPreference preference)
        {
            CurrentNotificationPreference = preference;
            hotlineManager.ReportNowAsync()
                .ContinueWithErrorHandling();
        }

        public void StartCovertDistress()
        {
            Log.Trace("Request for covert distress");

            lock (syncRoot)
            {
                StartCovertDistressUnsafe();
            }
        }

        public void StartDistress()
        {
            Log.Trace("Request for distress");

            lock (syncRoot)
            {
                StartDistressUnsafe();
            }
        }

        public void StartDistressCountdown(TimeSpan countdownTimeSpan)
        {
            Log.Trace("Request for countdown activation");

            lock (syncRoot)
            {
                StartDistressCountdownUnsafe(countdownTimeSpan);
            }
        }

        public void StartDistressCountdown()
        {
            StartDistressCountdown(CountdownInterval);
        }

        public void SaveState()
        {
            var current = GetCurrentState();
            if (IsCovertDistressOn || current != DistressState.Off)
            {
                var state = new DistressPersistenceState
                {
                    IsCovertOn = IsCovertDistressOn,
                    State = current,
                    DesiredCategory = DesiredCategory,
                    CurrentNotificationPreference = CurrentNotificationPreference,
                };

                if (current == DistressState.Countdown)
                {
                    state.NextCountdownExpiryTime = NextCountdownExpiryTime;
                }

                model.LocalSettings.LastDistressState = state;
            }
            else
            {
                model.LocalSettings.LastDistressState = null;
            }
        }

        public void RestoreState()
        {
            var state = model.LocalSettings.LastDistressState;
            if (state == null) return;

            if (state.IsCovertOn)
            {
                if (!IsCovertDistressOn)
                    StartCovertDistress();
            }
            else
            {
                if (IsCovertDistressOn)
                    EndDistress();
            }

            var current = GetCurrentState();

            if (state.State != DistressState.Off)
            {
                if (current != DistressState.On)
                {
                    if (state.State == DistressState.Countdown)
                    {
                        TimeSpan timeLeft;
                        if (state.NextCountdownExpiryTime.HasValue)
                        {
                            timeLeft = state.NextCountdownExpiryTime.Value - DateTimeOffset.Now;
                        }
                        else
                        {
                            timeLeft = TimeSpan.FromSeconds(5);
                        }

                        if (timeLeft.TotalSeconds > 2 && timeLeft.TotalSeconds < 7)
                        {
                            StartDistressCountdown(timeLeft);
                        }
                        else StartDistress();
                    }
                    else
                        StartDistress();
                }
            }
            else
            {
                if (current == DistressState.Off)
                    StopDistressVisibility();
            }

            if (state.DesiredCategory.HasValue && state.DesiredCategory.Value != DistressCategory.General)
            {
                DesiredCategory = state.DesiredCategory.Value;
            }

            if (state.CurrentNotificationPreference.HasValue)
                CurrentNotificationPreference = state.CurrentNotificationPreference.Value;
        }

        private void ResetCategoryStates()
        {
            categoryStates = new List<CategoryActivityState>
            {
                new CategoryActivityState {State = ActivityState.Inactive},
                new CategoryActivityState {State = ActivityState.Inactive},
                new CategoryActivityState {State = ActivityState.Inactive},
                new CategoryActivityState {State = ActivityState.Inactive},
            };
        }

        private void CancelDistressCountdownUnsafe()
        {
            if (currentState == DistressState.Countdown)
            {
                currentState = DistressState.Off;
                DisposeCountdownTimer();
                ViewModel.Messenger.Send(ViewActions.Distress.DeactivateCountdown);

                if (!IsCovertDistressOn)
                {
                    RedScoobyApi.Client.Distress.DeactivateCountdown(DateTimeOffset.Now)
                        .ContinueWithErrorHandling();

                    hotlineManager.AutoAdapt();
                }

                concernManager.Value.RefillFeedbackTimerIfPossible();

                Log.Trace("Countdown cancelled");
            }
        }

        private void DisposeCountdownTimer()
        {
            if (countdownTimerSubscription != null)
            {
                Log.Trace("Disposing countdown timer");

                countdownTimerSubscription.Dispose();
                countdownTimerSubscription = null;
            }
        }

        private void EndDistressUnsafe()
        {
            if (currentState == DistressState.On || IsCovertDistressOn)
            {
                StopDistressVisibility();
                IsCovertDistressOn = false;

                RedScoobyApi.Client.Distress.Deactivate(DateTimeOffset.Now)
                    .ContinueWithErrorHandling();

                hotlineManager.ResetPeriodicReporting();

                Log.Trace("Distress ended");
            }
        }

        private Task RequestCancelDistressCountdownUnsafe()
        {
            Log.Trace("Cancel distress request");

            if (currentState == DistressState.Countdown)
            {
                DisposeCountdownTimer();
                return feedbackManager.RequestFeedbackAsync(FeedbackManager.FeedbackContext.DistressCancellation,
                    WaitForCancellationFeedbackInterval)
                    .ContinueWith(t =>
                    {
                        t.HandleError(true);

                        var res = t.Result;

                        switch (res)
                        {
                            case FeedbackManager.FeedbackResult.Success:
                            {
                                lock (syncRoot)
                                {
                                    CancelDistressCountdownUnsafe();
                                }

                                break;
                            }
                            case FeedbackManager.FeedbackResult.FailCovertly:
                            {
                                lock (syncRoot)
                                {
                                    IsCovertDistressOn = true;
                                    CancelDistressCountdownUnsafe();
                                    StartCovertDistressUnsafe();
                                }
                                break;
                            }
                            case FeedbackManager.FeedbackResult.Suppressed:
                            {
                                Log.Error("Distress feedback was suppressed. Should've never happened");
                                break;
                            }

                            case FeedbackManager.FeedbackResult.Failure:
                            case FeedbackManager.FeedbackResult.NoFeedback:
                            default:
                            {
                                StartDistress();
                                break;
                            }
                        }
                    });
            }
            else if (currentState == DistressState.On)
            {
                return RequestEndDistress();
            }
            return TaskCache.Completed;
        }

        private Task RequestEndDistressUnsafe()
        {
            Log.Trace("End distress request");

            if (currentState == DistressState.On)
            {
                return feedbackManager.RequestFeedbackAsync(FeedbackManager.FeedbackContext.DistressDeactivation,
                    TimeSpan.MaxValue)
                    .ContinueWith(t =>
                    {
                        t.HandleError(true);

                        var res = t.Result;

                        switch (res)
                        {
                            case FeedbackManager.FeedbackResult.Success:
                            {
                                EndDistress();
                                break;
                            }
                            case FeedbackManager.FeedbackResult.FailCovertly:
                            {
                                StartCovertDistress();
                                break;
                            }
                            case FeedbackManager.FeedbackResult.Suppressed:
                            {
                                Log.Error("Distress feedback was suppressed. Should've never happened");
                                break;
                            }

                            case FeedbackManager.FeedbackResult.Failure:
                            case FeedbackManager.FeedbackResult.NoFeedback:
                            default:
                            {
                                break;
                            }
                        }
                    });
            }
            return TaskCache.Completed;
        }

        private void SetupCountdownTimer(TimeSpan timeSpan)
        {
            Log.Trace("Setup countdown timers for: " + timeSpan);

            DisposeCountdownTimer();
            NextCountdownExpiryTime = DateTimeOffset.Now.Add(timeSpan);
            countdownTimerSubscription = Observable.Timer(timeSpan).Subscribe(t =>
            {
                DisposeCountdownTimer();
                StartDistress();
            });
        }

        private void StartCovertDistressUnsafe()
        {
            if (!IsCovertDistressOn)
            {
                Log.Trace("Starting covert distress");

                IsCovertDistressOn = true;

                if (currentState == DistressState.Off)
                {
                    RedScoobyApi.Client.Distress.Activate(DateTimeOffset.Now)
                        .ContinueWithErrorHandling();
                }

                hotlineManager.SwitchReporting(HotlineManager.TrueReportingMode.CovertDistress,
                    PeriodicReportCovertDistressTimeInterval);
            }
            StopDistressVisibility();
        }

        private void StartDistressCountdownUnsafe(TimeSpan countdownTimeSpan)
        {
            if (currentState == DistressState.Off)
            {
                Log.Trace("Starting distress countdown");

                DesiredCategory = DistressCategory.General;
                currentState = DistressState.Countdown;

                ViewModel.Messenger.Send(ViewActions.Distress.ActivateCountdown);
                SetupCountdownTimer(countdownTimeSpan);

                if (!IsCovertDistressOn)
                {
                    RedScoobyApi.Client.Distress.ActivateCountdown(DateTimeOffset.Now)
                        .ContinueWithErrorHandling();

                    hotlineManager.SwitchReporting(HotlineManager.TrueReportingMode.Distress,
                        PeriodicReportDistressTimeInterval);
                }
            }
        }

        private void StartDistressUnsafe()
        {
            if (currentState != DistressState.On)
            {
                Log.Trace("Starting distress");

                DesiredCategory = DistressCategory.General;
                currentState = DistressState.On;

                concernManager.Value.EndConcern();

                ViewModel.Messenger.Send(ViewActions.Distress.TurnOn);

                if (!IsCovertDistressOn)
                {
                    RedScoobyApi.Client.Distress.Activate(DateTimeOffset.Now)
                        .ContinueWithErrorHandling();

                    hotlineManager.SwitchReporting(HotlineManager.TrueReportingMode.Distress,
                        PeriodicReportDistressTimeInterval);
                }
            }
        }

        private void StopDistressVisibility()
        {
            Log.Trace("Ending distress visibility");
            if (currentState == DistressState.Countdown)
            {
                ViewModel.Messenger.Send(ViewActions.Distress.DeactivateCountdown);
            }
            else if (currentState == DistressState.On)
            {
                ViewModel.Messenger.Send(ViewActions.Distress.TurnOff);
            }
            currentState = DistressState.Off;
            ResetCategoryStates();
        }

        public class CategoryActivityState
        {
            public ActivityState State { get; set; }
            public DateTimeOffset DateTimeOffset { get; set; }
        }
    }

    public enum DistressState
    {
        Off,
        Countdown,
        On,
    }

    [Flags]
    public enum DistressCategory
    {
        General = 0,
        Defence = 1,
        Medical = 2,
        Accident = 3,
    }

    public enum DistressNotificationPreference
    {
        Auto,
        Circles,
        Professionals,
        CirclesAndProfessionals,
        Custom
    }
}
