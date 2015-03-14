// Author: Prasanna V. Loganathar
// Created: 6:48 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedScooby.Infrastructure.Utilities
{
    public class MorseCodeEmitter
    {
        public const string SosCode = "...---...";
        private readonly int unitTimeMillis;
        private readonly int unitTime3Millis;
        private readonly Func<Task> signalStartAction;
        private readonly Func<Task> signalEndAction;

        public MorseCodeEmitter(TimeSpan unitTime, Func<Task> signalStartAction, Func<Task> signalEndAction)
        {
            if (signalStartAction == null) throw new ArgumentNullException("signalStartAction");
            if (signalEndAction == null) throw new ArgumentNullException("signalEndAction");

            unitTimeMillis = (int) unitTime.TotalMilliseconds;
            unitTime3Millis = unitTimeMillis*3;
            this.signalStartAction = signalStartAction;
            this.signalEndAction = signalEndAction;
        }

        public async Task EmitCodeAsync(string code, CancellationToken token)
        {
            foreach (var c in code)
            {
                if (token.IsCancellationRequested) break;

                if (c == '.')
                    await SendSignalAsync(unitTimeMillis);
                else if (c == '-')
                    await SendSignalAsync(unitTime3Millis);
            }
        }

        public async Task LoopCodeAsync(string code, int segmentDelayMillis, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await EmitCodeAsync(code, token);
                if (!token.IsCancellationRequested)
                    try { await Task.Delay(segmentDelayMillis, token); }
                    catch (TaskCanceledException) { }
            }
        }

        private async Task SendSignalAsync(int millis)
        {
            await signalStartAction();
            await Task.Delay(millis);
            await signalEndAction();
            await Task.Delay(unitTimeMillis);
        }
    }
}
