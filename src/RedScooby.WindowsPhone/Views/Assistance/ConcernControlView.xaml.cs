// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Threading.Tasks;
using RedScooby.Actions;
using RedScooby.Components;
using RedScooby.Helpers;
using RedScooby.ViewModels.Assistance;
using RedScooby.Views.Components;

namespace RedScooby.Views.Assistance
{
    public class ConcernControlViewBase : UserControlView<ConcernControlViewModel> { }

    public partial class ConcernControlView
    {
        private ConcernState currentViewState;

        public ConcernControlView()
        {
            InitializeComponent();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            SubscribeTo(ViewActions.Concern.TurnOn, async () => { await ActivateActiveConcernView(); });
            SubscribeTo(ViewActions.Concern.TurnOff, async () => { await DeactivateActiveConcernView(); });

            SetCurrentState();
        }

        private void SetCurrentState()
        {
            var pastViewState = currentViewState;
            var newState = Model.CurrentConcernState;
            if (pastViewState == newState) return;
            var ignore = newState == ConcernState.On ? ActivateActiveConcernView() : DeactivateActiveConcernView();
        }

        private async Task ActivateActiveConcernView()
        {
            currentViewState = ConcernState.On;
            ConcernActiveViewHolder.Activate(true, 0);
            await ConcernActiveStoryboard.RunAsync();
        }

        private async Task DeactivateActiveConcernView()
        {
            currentViewState = ConcernState.Off;
            await ConcernActiveStoryboard.RunReverseAsync();
            ConcernActiveViewHolder.Deactivate(false);
        }
    }
}
