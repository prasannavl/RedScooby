// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedScooby.Helpers
{
    public static class InterlockedHelpers
    {
        public static void SpinWaitUntilCompareExchangeSucceeds(ref int location, int value, int comparand)
        {
            if (Interlocked.CompareExchange(ref location, value, comparand) == comparand) return;
            var spinWait = new SpinWait();
            do
            {
                spinWait.SpinOnce();
            } while (Interlocked.CompareExchange(ref location, value, comparand) != comparand);
        }

        public static void SpinWaitUntilCompareExchangeSucceeds<T>(ref T location, T value, T comparand) where T : class
        {
            if (Interlocked.CompareExchange(ref location, value, comparand) == comparand) return;
            var spinWait = new SpinWait();
            do
            {
                spinWait.SpinOnce();
            } while (Interlocked.CompareExchange(ref location, value, comparand) != comparand);
        }

        public static void SpinWaitUntilCompareExchangeSucceeds<T>(ref T location, Func<T> func, T comparand)
            where T : class
        {
            if (Interlocked.CompareExchange(ref location, func(), comparand) == comparand) return;
            var spinWait = new SpinWait();
            do
            {
                spinWait.SpinOnce();
            } while (Interlocked.CompareExchange(ref location, func(), comparand) != comparand);
        }
    }

    public class SharedAwaitableOperation
    {
        private readonly Func<Task> operation;
        private readonly TaskCompletionSourceWrapper tcsWrapper;
        private InterlockedBlockingMonitor monitor;

        public SharedAwaitableOperation(Func<Task> operationDelegate)
        {
            if (operation == null) throw new ArgumentNullException("operationDelegate");

            tcsWrapper = new TaskCompletionSourceWrapper();
            operation = operationDelegate;
            monitor = new InterlockedBlockingMonitor();
        }

        public async Task RunAsync()
        {
            monitor.Enter();
            var tcs = tcsWrapper.Value;
            if (tcs != null)
            {
                monitor.Exit();
                await tcs.Task;
                return;
            }

            tcs = tcsWrapper.Value = new TaskCompletionSource<bool>();
            monitor.Exit();

            await operation();

            monitor.Enter();
            tcsWrapper.Value = null;
            monitor.Exit();
            tcs.TrySetResult(true);
        }
    }

    internal class HybridInterlockedMonitor
    {
        private int busy;
        private InterlockedBlockingMonitor countMonitor = new InterlockedBlockingMonitor();
        private int waitersCount;
        public SemaphoreSlim UnderlyingSemaphore { get; private set; }

        public bool IsBusy
        {
            get { return Interlocked.CompareExchange(ref busy, -1, -1) > 0; }
        }

        private SemaphoreSlim SemaphoreSlim
        {
            get { return UnderlyingSemaphore ?? (UnderlyingSemaphore = new SemaphoreSlim(0, 1)); }
        }

        public void Enter()
        {
            while (!TryEnter())
            {
                AddWaiterCount();
                if (TryEnter())
                {
                    RemoveWaiterCount();
                    return;
                }
                SemaphoreSlim.Wait();
                RemoveWaiterCount();
            }
        }

        public async Task EnterAsync()
        {
            while (!TryEnter())
            {
                AddWaiterCount();
                if (TryEnter())
                {
                    RemoveWaiterCount();
                    return;
                }
                await SemaphoreSlim.WaitAsync();
                RemoveWaiterCount();
            }
        }

        public bool TryEnter()
        {
            return Interlocked.CompareExchange(ref busy, 1, 0) == 0;
        }

        public void Exit()
        {
            countMonitor.Enter();
            Interlocked.Exchange(ref busy, 0);
            try
            {
                if (waitersCount > 0) SemaphoreSlim.Release();
            }
            finally
            {
                countMonitor.Exit();
            }
        }

        private void AddWaiterCount()
        {
            countMonitor.Enter();
            waitersCount++;
            countMonitor.Exit();
        }

        private void RemoveWaiterCount()
        {
            countMonitor.Enter();
            waitersCount--;
            countMonitor.Exit();
        }
    }

    internal struct InterlockedBlockingMonitor
    {
        private int busy;

        public bool IsBusy
        {
            get { return Interlocked.CompareExchange(ref busy, -1, -1) > 0; }
        }

        /// <summary>
        ///     WARNING: This kind of locking is not to be used with awaits in-between Entry and Exit.
        ///     Task continuations expected to be run on the same thread (eg: UI context) could result in a deadlock.
        ///     This is also the reason why this functionality is not a part of the InterlockedMonitor itself, and is
        ///     isolated. (The same reason compiler disallows await within locks)
        /// </summary>
        public void Enter()
        {
            InterlockedHelpers.SpinWaitUntilCompareExchangeSucceeds(ref busy, 1, 0);
        }

        public void Exit()
        {
            Interlocked.Exchange(ref busy, 0);
        }
    }

    internal struct InterlockedMonitor
    {
        private int busy;

        public bool IsBusy
        {
            get { return Interlocked.CompareExchange(ref busy, -1, -1) > 0; }
        }

        public bool TryEnter()
        {
            return Interlocked.CompareExchange(ref busy, 1, 0) == 0;
        }

        public void Exit()
        {
            Interlocked.Exchange(ref busy, 0);
        }
    }
}
