// Author: Prasanna V. Loganathar
// Created: 9:28 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Flashlight;
using RedScooby.Infrastructure.Framework.Commands;

namespace RedScooby.ViewModels.Fragments
{
    public class FlashlightViewModel : ViewModelBase
    {
        public const string SwitchToSOSText = "Switch to SOS";
        public const string FlashlightText = "Flashlight";
        public const string FlashlightOffText = "Turn off flashlight";
        private readonly FlashlightControl control;
        private string flashlightNextStateText;

        public FlashlightViewModel(FlashlightControl control)
        {
            this.control = control;

            SubscribeTo(FlashlightControl.FlashlightState.On, () => { FlashlightNextStateText = SwitchToSOSText; });

            SubscribeTo(FlashlightControl.FlashlightState.Sos,
                () => { FlashlightNextStateText = FlashlightOffText; });

            SubscribeTo(FlashlightControl.FlashlightState.Off, () => { FlashlightNextStateText = FlashlightText; });

            SetCurrentStateText();

            ToggleAsyncCommand = CommandFactory.CreateAsync(control.ToggleAsync);
        }

        public string FlashlightNextStateText
        {
            get { return flashlightNextStateText; }
            private set { SetAndNotifyIfChanged(ref flashlightNextStateText, value); }
        }

        public AsyncRelayCommand ToggleAsyncCommand { get; private set; }

        private void SetCurrentStateText()
        {
            var currentState = control.CurrentState;
            switch (currentState)
            {
                case FlashlightControl.FlashlightState.Off:
                    FlashlightNextStateText = FlashlightText;
                    break;
                case FlashlightControl.FlashlightState.On:
                    FlashlightNextStateText = SwitchToSOSText;
                    break;
                case FlashlightControl.FlashlightState.Sos:
                    FlashlightNextStateText = FlashlightOffText;
                    break;
            }
        }
    }
}
