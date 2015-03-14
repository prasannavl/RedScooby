// Author: Prasanna V. Loganathar
// Created: 9:16 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Actions;
using RedScooby.Components;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;

namespace RedScooby.ViewModels.Assistance
{
    public class ConcernControlViewModel : ViewModelBase
    {
        private readonly ConcernManager concernManager;

        public ConcernControlViewModel(ConcernManager concernManager)
        {
            this.concernManager = concernManager;
            StartConcernCommand = CommandFactory.Create(() =>
            {
                concernManager.SetNotificationPreference(ConcernNotificationPreference.Auto);
                concernManager.StartConcern(true);
            });

            StartPrecautionCommand = CommandFactory.Create(() =>
            {
                concernManager.SetNotificationPreference(ConcernNotificationPreference.Nobody);
                concernManager.StartConcern();
            });

            ForwardMessageToView(ViewActions.Concern.TurnOn);
        }

        public ConcernState CurrentConcernState
        {
            get { return concernManager.GetCurrentState(); }
        }

        public RelayCommand StartPrecautionCommand { get; private set; }
        public RelayCommand StartConcernCommand { get; private set; }
    }
}
