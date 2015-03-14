// Author: Prasanna V. Loganathar
// Created: 7:14 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RedScooby.Actions;
using RedScooby.Data.Tables;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Composition;
using RedScooby.Models;

namespace RedScooby.Components
{
    public class FeedbackManager
    {
        public enum FeedbackContext
        {
            ConcernPeriodic,
            ConcernDeactivation,
            DistressCancellation,
            DistressDeactivation,
            Custom,
        }

        public enum FeedbackResult
        {
            NoFeedback,
            Success,
            Failure,
            FailCovertly,
            Suppressed,
        }

        private readonly AppModel model;
        private bool feedbackInProgress;
        private InterlockedBlockingMonitor monitor = new InterlockedBlockingMonitor();
        private FeedbackRequest currentRequestInfo;

        public FeedbackManager(AppModel model)
        {
            this.model = model;
        }

        public FeedbackRequest CurrentRequestInfo
        {
            get
            {
                monitor.Enter();
                var req = currentRequestInfo;
                monitor.Exit();
                return req;
            }

            private set
            {
                DisposeFeedbackRequest();
                currentRequestInfo = value;
            }
        }

        public void SaveState()
        {
            var req = CurrentRequestInfo;
            if (req != null)
                model.LocalSettings.LastFeedbackState =
                    new FeedbackPersistenceState
                    {
                        Context = req.Context,
                        ExpiryDuration = req.ExpiryDuration,
                        StartTime = req.StartTime,
                        FeedbackInProgress = req.CompletionSource.Task.Status == TaskStatus.WaitingForActivation,
                    };
            else
                model.LocalSettings.LastFeedbackState = null;
        }

        public void RestoreState()
        {
            if (CurrentRequestInfo != null)
                return;

            var req = model.LocalSettings.LastFeedbackState;
            if (req != null && req.FeedbackInProgress)
            {
                TimeSpan timeLeft;
                timeLeft = req.ExpiryDuration == TimeSpan.MaxValue
                    ? req.ExpiryDuration
                    : req.StartTime.Add(req.ExpiryDuration).Subtract(DateTimeOffset.Now);

                if (timeLeft.TotalSeconds > 0)
                {
                    DistressManager.HintDistressPossibility(model, true);
                }
                else
                {
                    DistressManager.HintDistressPossibility(model);
                }
            }
        }

        public Task<FeedbackResult> RequestFeedbackAsync(FeedbackContext context, TimeSpan timeSpan)
        {
            monitor.Enter();
            if (feedbackInProgress)
            {
                monitor.Exit();
                return Task.FromResult(FeedbackResult.Suppressed);
            }

            feedbackInProgress = true;
            var req = new FeedbackRequest(context, timeSpan, true);
            req.CompletionSource.Task.ContinueWith(t =>
            {
                monitor.Enter();
                CurrentRequestInfo = null;
                feedbackInProgress = false;
                monitor.Exit();
                ViewModel.Messenger.Send(ViewActions.PinFeedback.Hide);
            });

            CurrentRequestInfo = req;
            monitor.Exit();

            ViewModel.Messenger.Send(ViewActions.PinFeedback.Show);
            return req.CompletionSource.Task;
        }

        private void DisposeFeedbackRequest()
        {
            var req = currentRequestInfo;
            currentRequestInfo = null;
            if (req != null)
                req.Dispose();
        }

        public class FeedbackRequest : IDisposable
        {
            private IDisposable timer;
            private bool isResultSet;
            private int invalidCodeCount;

            [JsonConstructor]
            internal FeedbackRequest() { }

            internal FeedbackRequest(FeedbackContext context, TimeSpan expiryDuration, bool autoStartTimers = false)
            {
                CompletionSource = new TaskCompletionSource<FeedbackResult>();
                Context = context;
                ExpiryDuration = expiryDuration;

                if (autoStartTimers)
                    StartTimerIfRequired();
            }

            public TaskCompletionSource<FeedbackResult> CompletionSource { get; set; }
            public FeedbackContext Context { get; set; }
            public DateTimeOffset StartTime { get; set; }
            public TimeSpan ExpiryDuration { get; set; }

            public void Dispose()
            {
                if (!isResultSet)
                    SetResult(FeedbackResult.NoFeedback);
            }

            public void StartTimerIfRequired()
            {
                if (timer == null && ExpiryDuration != TimeSpan.MaxValue)
                {
                    StartTime = DateTimeOffset.Now;
                    timer =
                        Observable.Timer(ExpiryDuration)
                            .Subscribe(x => { SetResult(FeedbackResult.NoFeedback); });
                }
            }

            public bool HandleInvalidCode()
            {
                invalidCodeCount++;
                if (invalidCodeCount == 3)
                {
                    DisposeTimer();
                    SetResult(FeedbackResult.Failure);
                    return true;
                }

                return false;
            }

            public void SetResult(FeedbackResult result)
            {
                if (!isResultSet)
                {
                    isResultSet = true;
                    DisposeTimer();
                    CompletionSource.TrySetResult(result);
                }
            }

            private void DisposeTimer()
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
            }
        }
    }
}
