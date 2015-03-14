// Author: Prasanna V. Loganathar
// Created: 6:48 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using LiquidState;
using LiquidState.Machines;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Utilities;

namespace RedScooby.Infrastructure.Flashlight
{
    public class FlashlightControl
    {
        public enum FlashlightState
        {
            On,
            Off,
            Sos,
        }

        public enum FlashlightTrigger
        {
            TurnOn,
            SwitchToSos,
            TurnOff,
        }

        private readonly IAwaitableStateMachine<FlashlightState, FlashlightTrigger> stateMachine;
        private readonly IFlashlightService flashlightService;
        private Tuple<Task, IDisposable> stateMachineSosLoop;

        public FlashlightControl(IFlashlightService flashlightService)
        {
            this.flashlightService = flashlightService;

            var config = StateMachine.CreateAwaitableConfiguration<FlashlightState, FlashlightTrigger>();

            config.ForState(FlashlightState.Off)
                .OnEntry(async () =>
                {
                    await this.flashlightService.TurnOff();
                    if (!flashlightService.IsFastSwitchingCapable)
                        flashlightService.Release();
                    ViewModel.Messenger.Send(FlashlightState.Off);
                })
                .Permit(FlashlightTrigger.TurnOn, FlashlightState.On)
                .Permit(FlashlightTrigger.SwitchToSos, FlashlightState.Sos)
                .Ignore(FlashlightTrigger.TurnOff);

            config.ForState(FlashlightState.On)
                .OnEntry(async () =>
                {
                    await this.flashlightService.TurnOn();
                    ViewModel.Messenger.Send(FlashlightState.On);
                })
                .Permit(FlashlightTrigger.TurnOff, FlashlightState.Off)
                .Permit(FlashlightTrigger.SwitchToSos, FlashlightState.Sos)
                .Ignore(FlashlightTrigger.TurnOn);

            config.ForState(FlashlightState.Sos)
                .OnEntry(async () =>
                {
                    if (CurrentState != FlashlightState.Off)
                    {
                        await this.flashlightService.TurnOff();
                    }

                    stateMachineSosLoop = StartSosLoop();
                    ViewModel.Messenger.Send(FlashlightState.Sos);
                })
                .OnExit(async () =>
                {
                    if (stateMachineSosLoop != null)
                    {
                        stateMachineSosLoop.Item2.Dispose();
                        await stateMachineSosLoop.Item1;
                    }
                })
                .Permit(FlashlightTrigger.TurnOff, FlashlightState.Off)
                .Permit(FlashlightTrigger.TurnOn, FlashlightState.On)
                .Ignore(FlashlightTrigger.SwitchToSos);

            stateMachine = StateMachine.Create(FlashlightState.Off, config);
        }

        public FlashlightState CurrentState
        {
            get { return stateMachine.CurrentState; }
        }

        public async Task ReleaseFlashlightAsync()
        {
            if (CurrentState != FlashlightState.Off)
            {
                await SwitchOffAsync();
            }
            flashlightService.Release();
        }

        public Task<bool> IsAvailableAsync()
        {
            return flashlightService.IsAvailableAsync();
        }

        public Task SwitchOffAsync()
        {
            return stateMachine.FireAsync(FlashlightTrigger.TurnOff);
        }

        public Task SwitchOnAsync()
        {
            return stateMachine.FireAsync(FlashlightTrigger.TurnOn);
        }

        public Task SwitchToSosLoopAsync()
        {
            return stateMachine.FireAsync(FlashlightTrigger.SwitchToSos);
        }

        public async Task ToggleAsync()
        {
            if (await IsAvailableAsync())
            {
                switch (CurrentState)
                {
                    case FlashlightState.Off:
                        await stateMachine.FireAsync(FlashlightTrigger.TurnOn);
                        return;
                    case FlashlightState.On:
                        await stateMachine.FireAsync(FlashlightTrigger.SwitchToSos);
                        return;
                    case FlashlightState.Sos:
                        await stateMachine.FireAsync(FlashlightTrigger.TurnOff);
                        return;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        internal async Task MoveToStateAsync(FlashlightState state)
        {
            switch (state)
            {
                case FlashlightState.Off:
                    await stateMachine.FireAsync(FlashlightTrigger.TurnOff);
                    return;
                case FlashlightState.On:
                    await stateMachine.FireAsync(FlashlightTrigger.TurnOn);
                    return;
                case FlashlightState.Sos:
                    await stateMachine.FireAsync(FlashlightTrigger.SwitchToSos);
                    return;
                default:
                    throw new InvalidOperationException();
            }
        }

        private Tuple<Task, IDisposable> StartSosLoop()
        {
            var morseCodeEmitter = new MorseCodeEmitter(
                TimeSpan.FromMilliseconds(100),
                () => flashlightService.TurnOn(),
                () => flashlightService.TurnOff());

            var sosCancellationTokenSource = new CancellationTokenSource();
            var task =
                Task.Run(
                    () =>
                        morseCodeEmitter.LoopCodeAsync(MorseCodeEmitter.SosCode, 500, sosCancellationTokenSource.Token));
            return new Tuple<Task, IDisposable>(task, new CancellationDisposable(sosCancellationTokenSource));
        }
    }
}
