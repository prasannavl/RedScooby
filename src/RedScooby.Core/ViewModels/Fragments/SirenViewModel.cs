// Author: Prasanna V. Loganathar
// Created: 9:28 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.Infrastructure.Siren;

namespace RedScooby.ViewModels.Fragments
{
    public class SirenViewModel : ViewModelBase
    {
        private const string TurnedOnText = "Turn off siren";
        private const string NormalText = "Siren";
        private readonly SirenControl control;
        private string sirenNextStateText;

        public SirenViewModel(SirenControl control)
        {
            this.control = control;

            SubscribeTo(SirenControl.SirenState.On, () => { SirenNextStateText = TurnedOnText; });
            SubscribeTo(SirenControl.SirenState.Off, () => { SirenNextStateText = NormalText; });

            UpdateText();

            ToggleAsyncCommand = CommandFactory.CreateAsync(control.Toggle);
        }

        public string SirenNextStateText
        {
            get { return sirenNextStateText; }
            private set { SetAndNotifyIfChanged(ref sirenNextStateText, value); }
        }

        public AsyncRelayCommand ToggleAsyncCommand { get; private set; }

        private void UpdateText()
        {
            var currentState = control.GetState();
            SirenNextStateText = currentState == SirenControl.SirenState.On ? TurnedOnText : NormalText;
        }
    }
}
