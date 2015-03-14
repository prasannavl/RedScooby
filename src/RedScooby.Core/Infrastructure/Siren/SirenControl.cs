// Author: Prasanna V. Loganathar
// Created: 6:48 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Threading.Tasks;
using LiquidState;
using LiquidState.Common;
using LiquidState.Machines;
using RedScooby.Infrastructure.Composition;

namespace RedScooby.Infrastructure.Siren
{
    public class SirenControl
    {
        public enum SirenState
        {
            On,
            Off,
        }

        public enum SirenTrigger
        {
            TurnOn,
            TurnOff,
        }

        private readonly ISirenService sirenService;
        private readonly IAwaitableStateMachine<SirenState, SirenTrigger> stateMachine;

        public SirenControl(ISirenService sirenService)
        {
            this.sirenService = sirenService;

            var config = StateMachine.CreateAwaitableConfiguration<SirenState, SirenTrigger>();
            config.ForState(SirenState.Off)
                .OnEntry(async () =>
                {
                    await sirenService.TurnOffAsync();
                    ViewModel.Messenger.Send(SirenState.Off);
                })
                .Ignore(SirenTrigger.TurnOff)
                .Permit(SirenTrigger.TurnOn, SirenState.On);

            config.ForState(SirenState.On)
                .OnEntry(async () =>
                {
                    await sirenService.TurnOnAsync();
                    ViewModel.Messenger.Send(SirenState.On);
                })
                .Ignore(SirenTrigger.TurnOn)
                .Permit(SirenTrigger.TurnOff, SirenState.Off);

            stateMachine = StateMachine.Create(SirenState.Off, config);
        }

        public async Task Toggle()
        {
            switch (stateMachine.CurrentState)
            {
                case SirenState.Off:
                    await TurnOnAsync();
                    break;
                case SirenState.On:
                    await TurnOffAsync();
                    break;
            }
        }

        public SirenState GetState()
        {
            return stateMachine.CurrentState;
        }

        public async Task TurnOnAsync()
        {
            await stateMachine.FireAsync(SirenTrigger.TurnOn);
        }

        public async Task TurnOffAsync()
        {
            await stateMachine.FireAsync(SirenTrigger.TurnOff);
        }

        public async Task SyncStateAsync()
        {
            var state = GetState();
            if (sirenService.IsOn)
            {
                if (state == SirenState.Off)
                {
                    await stateMachine.MoveToState(SirenState.On, StateTransitionOption.SkipAllTransitions);
                    ViewModel.Messenger.Send(SirenState.On);
                }
            }
            else
            {
                if (state == SirenState.On)
                {
                    await stateMachine.MoveToState(SirenState.Off, StateTransitionOption.SkipAllTransitions);
                    ViewModel.Messenger.Send(SirenState.Off);
                }
            }
        }
    }
}
