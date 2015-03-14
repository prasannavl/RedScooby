// Author: Prasanna V. Loganathar
// Created: 9:16 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using System.Threading.Tasks;
using RedScooby.Actions;
using RedScooby.Components;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.ViewModels.Fragments;

namespace RedScooby.ViewModels.Assistance
{
    public class AssistanceViewModel : ViewModelBase
    {
        private readonly FlashlightViewModel flashlightViewModel;
        private readonly SirenViewModel sirenViewModel;
        private readonly ConcernManager concernManager;
        private readonly DistressManager distressManager;
        private readonly FeedbackManager feedbackManager;

        public AssistanceViewModel(FlashlightViewModel flashlightViewModel, SirenViewModel sirenViewModel,
            ConcernManager concernManager, DistressManager distressManager, FeedbackManager feedbackManager)
        {
            this.flashlightViewModel = flashlightViewModel;
            this.sirenViewModel = sirenViewModel;
            this.concernManager = concernManager;
            this.distressManager = distressManager;
            this.feedbackManager = feedbackManager;

            MapNotificationsFrom(flashlightViewModel, new KeyValuePair<string, string>(
                ExpressionHelpers.GetMemberName(() => flashlightViewModel.FlashlightNextStateText),
                ExpressionHelpers.GetMemberName(() => FlashlightSwitchText)), true);

            MapNotificationsFrom(sirenViewModel, new KeyValuePair<string, string>(
                ExpressionHelpers.GetMemberName(() => sirenViewModel.SirenNextStateText),
                ExpressionHelpers.GetMemberName(() => SirenSwitchText)), true);

            ForwardMessageToView(ViewActions.PinFeedback.Show);
            ForwardMessageToView(ViewActions.PinFeedback.Hide);

            ForwardMessageToView(ViewActions.Distress.TurnOn);

            SubscribeTo(ViewActions.Distress.TurnOff, DeactiveDistress);

            ToggleFlashlightAsyncCommand = flashlightViewModel.ToggleAsyncCommand;
            ToggleSirenAsyncCommand = sirenViewModel.ToggleAsyncCommand;
        }

        public DistressState CurrentDistressState
        {
            get { return distressManager.GetCurrentState(); }
        }

        public bool PinFeedbackViewStateVisible
        {
            get
            {
                var req = feedbackManager.CurrentRequestInfo;
                if (req == null)
                    return false;

                return req.CompletionSource.Task.Status == TaskStatus.WaitingForActivation;
            }
        }

        public string FlashlightSwitchText
        {
            get { return flashlightViewModel.FlashlightNextStateText; }
        }

        public string SirenSwitchText
        {
            get { return sirenViewModel.SirenNextStateText; }
        }

        public AsyncRelayCommand ToggleFlashlightAsyncCommand { get; private set; }
        public AsyncRelayCommand ToggleSirenAsyncCommand { get; private set; }

        private void DeactiveDistress()
        {
            ViewMessenger.Send(ViewActions.Distress.TurnOff);
            ViewMessenger.Send(ViewActions.Distress.DeactivateCountdown);
        }
    }
}
