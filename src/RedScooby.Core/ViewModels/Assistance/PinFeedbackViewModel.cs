// Author: Prasanna V. Loganathar
// Created: 9:16 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Linq;
using RedScooby.Actions;
using RedScooby.Common.Resources;
using RedScooby.Components;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.Models;

namespace RedScooby.ViewModels.Assistance
{
    public class PinFeedbackViewModel : ViewModelBase
    {
        private readonly AppModel appModel;
        private FeedbackManager.FeedbackRequest feedbackRequest;
        private string feedbackInfoText;
        private string feedbackInfoTextTemplate;
        private double feedbackSecondsLeft;
        private IDisposable timer;
        private string leftButtonText;
        private string enteredPinCode;
        private bool isInvalid;

        public PinFeedbackViewModel(AppModel appModel, FeedbackManager feedbackManager)
        {
            this.appModel = appModel;
            LeftButtonCommand = CommandFactory.Create(() =>
            {
                if (feedbackRequest != null)
                    feedbackRequest.Dispose();
                else
                    ViewMessenger.Send(ViewActions.PinFeedback.Hide);
            });

            BackspaceCommand = CommandFactory.Create(() =>
            {
                var code = EnteredPinCode;
                if (!string.IsNullOrEmpty(code))
                {
                    EnteredPinCode = code.Substring(0, code.Length - 1);
                }
            });

            SetupForRequest(feedbackManager.CurrentRequestInfo);
        }

        public string FeedbackInfoText
        {
            get { return feedbackInfoText; }
            set { SetAndNotifyIfChanged(ref feedbackInfoText, value); }
        }

        public string LeftButtonText
        {
            get { return leftButtonText; }
            set { SetAndNotifyIfChanged(ref leftButtonText, value); }
        }

        public string EnteredPinCode
        {
            get { return enteredPinCode; }
            set { SetAndNotifyIfChanged(ref enteredPinCode, value); }
        }

        public bool IsPinInvalid
        {
            get { return isInvalid; }
            set { SetAndNotifyIfChanged(ref isInvalid, value); }
        }

        public RelayCommand LeftButtonCommand { get; private set; }
        public RelayCommand BackspaceCommand { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            DisposeTimer();
            if (feedbackRequest != null)
                feedbackRequest.Dispose();
        }

        public void AddToInput(string s)
        {
            var prevLength = 0;
            var prevCode = EnteredPinCode;
            if (!string.IsNullOrEmpty(prevCode))
            {
                prevLength = prevCode.Length;
            }
            var currentLength = s.Length;
            var newLength = prevLength + currentLength;
            var code = prevCode + s;

            if (feedbackRequest != null)
            {
                var userSecurityInfo = appModel.User.Settings;
                if (feedbackRequest.Context == FeedbackManager.FeedbackContext.ConcernPeriodic)
                {
                    if (newLength > 3)
                        return;


                    if (newLength == 3)
                    {
                        if (code == userSecurityInfo.ThreeDigitDisasterPinCode)
                        {
                            EnteredPinCode = string.Empty;
                            feedbackRequest.SetResult(FeedbackManager.FeedbackResult.FailCovertly);
                        }
                        else if (code == userSecurityInfo.ThreeDigitPinCode)
                        {
                            EnteredPinCode = string.Empty;
                            feedbackRequest.SetResult(FeedbackManager.FeedbackResult.Success);
                        }
                        else
                        {
                            EnteredPinCode = code;
                            IsPinInvalid = true;

                            if (feedbackRequest.HandleInvalidCode())
                                return;
                            Observable.Timer(TimeSpan.FromMilliseconds(400)).Subscribe(x =>
                            {
                                EnteredPinCode = string.Empty;
                                IsPinInvalid = false;
                            });
                        }

                        return;
                    }
                    else
                    {
                        EnteredPinCode = code;
                        return;
                    }
                }
                else
                {
                    if (newLength > 4)
                        return;


                    if (newLength == 4)
                    {
                        if (code == userSecurityInfo.DisasterPinCode)
                        {
                            EnteredPinCode = string.Empty;
                            feedbackRequest.SetResult(FeedbackManager.FeedbackResult.FailCovertly);
                        }
                        else if (code == userSecurityInfo.PinCode)
                        {
                            EnteredPinCode = string.Empty;
                            feedbackRequest.SetResult(FeedbackManager.FeedbackResult.Success);
                        }
                        else
                        {
                            IsPinInvalid = true;
                            EnteredPinCode = code;

                            if (feedbackRequest.HandleInvalidCode())
                                return;

                            Observable.Timer(TimeSpan.FromMilliseconds(350)).Subscribe(x =>
                            {
                                IsPinInvalid = false;
                                EnteredPinCode = string.Empty;
                            });
                        }

                        return;
                    }
                    else
                    {
                        EnteredPinCode = code;
                        return;
                    }
                }
            }
        }

        private void SetupForRequest(FeedbackManager.FeedbackRequest request)
        {
            feedbackRequest = request;
            if (request == null)
            {
                UpdateInfoText();
                return;
            }

            if (request.StartTime != DateTimeOffset.MinValue)
            {
                feedbackSecondsLeft = (request.StartTime.Add(request.ExpiryDuration) - DateTimeOffset.Now).TotalSeconds;
            }
            if (feedbackSecondsLeft < 0) feedbackSecondsLeft = 0;

            feedbackInfoTextTemplate = RetriveTextTemplate();
            UpdateInfoText();
            SetupTimer();
        }

        private void SetupTimer()
        {
            DisposeTimer();
            if (feedbackSecondsLeft > 0)
            {
                timer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                    .Subscribe(t =>
                    {
                        feedbackSecondsLeft--;
                        if (feedbackSecondsLeft < 0)
                        {
                            DisposeTimer();
                        }
                        UpdateInfoText();
                    });
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

        private void UpdateInfoText()
        {
            if (feedbackInfoTextTemplate == null)
                feedbackInfoTextTemplate = RetriveTextTemplate();

            FeedbackInfoText = string.Format(feedbackInfoTextTemplate, feedbackSecondsLeft.ToString("00"));
        }

        private string RetriveTextTemplate()
        {
            if (feedbackRequest == null)
                return CommonStrings.PinModeHeader;

            switch (feedbackRequest.Context)
            {
                case FeedbackManager.FeedbackContext.ConcernDeactivation:
                    return CommonStrings.PinModeConcernOff;
                case FeedbackManager.FeedbackContext.ConcernPeriodic:
                    return CommonStrings.PinModeConcernFeedback;
                case FeedbackManager.FeedbackContext.DistressCancellation:
                    return CommonStrings.PinModeDistressCancel;
                case FeedbackManager.FeedbackContext.DistressDeactivation:
                    return CommonStrings.PinModeDistressOff;
                default:
                    return CommonStrings.PinModeHeader;
            }
        }
    }
}
