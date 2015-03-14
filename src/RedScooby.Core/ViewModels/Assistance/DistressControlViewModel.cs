// Author: Prasanna V. Loganathar
// Created: 9:16 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using RedScooby.Actions;
using RedScooby.Components;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;

namespace RedScooby.ViewModels.Assistance
{
    public class DistressControlViewModel : ViewModelBase
    {
        private readonly DistressManager distressManager;

        public DistressControlViewModel(DistressManager distressManager)
        {
            this.distressManager = distressManager;
            ForwardMessageToView(ViewActions.Distress.ActivateCountdown);

            ActivateCountdownCommand = CommandFactory.Create(distressManager.StartDistressCountdown);
            ActivateCountdownForTapAndHoldCommand =
                CommandFactory.Create(() => distressManager.StartDistressCountdown(TimeSpan.FromSeconds(3)));
        }

        public DistressState CurrentDistressState
        {
            get { return distressManager.GetCurrentState(); }
        }

        public RelayCommand ActivateCountdownCommand { get; private set; }
        public RelayCommand ActivateCountdownForTapAndHoldCommand { get; private set; }
    }
}
