// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using RedScooby.Infrastructure.Framework;
using RedScooby.Logging;

namespace RedScooby.Helpers
{
    public static class TaskHelpers
    {
        public static Task ContinueWithResultPropagation(this Task task, TaskCompletionSource<bool> tcs)
        {
            return task
                .ContinueWith(t =>
                {
                    if (t.IsCanceled)
                        tcs.TrySetCanceled();
                    else if (t.IsFaulted)
                        tcs.TrySetException(t.Exception);
                    else
                        tcs.TrySetResult(true);
                });
        }

        public static Task ContinueWithResultPropagation<TResult>(this Task<TResult> task,
            TaskCompletionSource<TResult> tcs)
        {
            return task
                .ContinueWith(t =>
                {
                    if (t.IsCanceled)
                        tcs.TrySetCanceled();
                    else if (t.IsFaulted)
                        tcs.TrySetException(t.Exception);
                    else
                        tcs.TrySetResult(t.Result);
                });
        }

        public static Task ContinueWithLogCriticalOnException(this Task task, bool throwException = false)
        {
            return task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    var ex = t.Exception;
                    Log.Critical(f => f(ex));
                    if (throwException) ExceptionDispatchInfo.Capture(ex).Throw();
                }
            });
        }

        public static Task ContinueWithErrorHandling(this Task task, bool throwException = false)
        {
            return task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    var ex = t.Exception;
                    if (throwException)
                        ErrorHandler.Current.HandleError(ex);
                    else
                        ErrorHandler.Current.HandleSilentError(ex);
                }
            });
        }

        public static Task HandleError(this Task task, bool throwException = false)
        {
            if (task.IsFaulted)
            {
                var ex = task.Exception;
                if (throwException)
                    ErrorHandler.Current.HandleError(ex);
                else
                    ErrorHandler.Current.HandleSilentError(ex);
            }

            return task;
        }

        public static Task ContinueWithLogErrorOnException(this Task task, bool throwException = false)
        {
            return task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    var ex = t.Exception;
                    Log.Error(f => f(ex));
                    if (throwException) ExceptionDispatchInfo.Capture(ex).Throw();
                }
            });
        }

        public static Task LogCriticalOnException(this Task task, bool throwException = false)
        {
            if (task.IsFaulted)
            {
                var ex = task.Exception;
                Log.Critical(f => f(ex));
                if (throwException) ExceptionDispatchInfo.Capture(ex).Throw();
            }

            return task;
        }

        public static Task LogErrorOnException(this Task task, bool throwException = false)
        {
            if (task.IsFaulted)
            {
                var ex = task.Exception;
                Log.Error(f => f(ex));
                if (throwException) ExceptionDispatchInfo.Capture(ex).Throw();
            }

            return task;
        }

        public static Task ContinueOnCurrentContextWith(this Task task, Action<Task> continuationAction)
        {
            return task.ContinueOnCurrentContextWith(continuationAction, CancellationToken.None);
        }

        public static Task ContinueOnCurrentContextWith(this Task task, Action<Task> continuationAction,
            CancellationToken cancellationToken)
        {
            return task.ContinueWith(continuationAction, cancellationToken, TaskContinuationOptions.DenyChildAttach,
                SynchronizationContext.Current == null
                    ? TaskScheduler.Current
                    : TaskScheduler.FromCurrentSynchronizationContext());
        }

        public static Task ContinueOnCurrentContextWith<T>(this Task<T> task, Action<Task<T>> continuationAction)
        {
            return task.ContinueOnCurrentContextWith(continuationAction, CancellationToken.None);
        }

        public static Task ContinueOnCurrentContextWith<T>(this Task<T> task, Action<Task<T>> continuationAction,
            CancellationToken cancellationToken)
        {
            return task.ContinueWith(continuationAction, cancellationToken, TaskContinuationOptions.DenyChildAttach,
                SynchronizationContext.Current == null
                    ? TaskScheduler.Current
                    : TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    public class TaskCompletionSourceWrapper
    {
        public TaskCompletionSource<bool> Value { get; set; }
    }

    public static class AsyncHelpers
    {
        public static void RunSync(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            synch.Post(async _ =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }

        public static T RunSync<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            var ret = default(T);
            synch.Post(async _ =>
            {
                try
                {
                    ret = await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);
            return ret;
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);

            private readonly Queue<Tuple<SendOrPostCallback, object>> items =
                new Queue<Tuple<SendOrPostCallback, object>>();

            private bool done;
            public Exception InnerException { get; set; }

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (items)
                {
                    items.Enqueue(Tuple.Create(d, state));
                }
                workItemsWaiting.Set();
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }

            public void EndMessageLoop()
            {
                Post(_ => done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (items)
                    {
                        if (items.Count > 0)
                        {
                            task = items.Dequeue();
                        }
                    }
                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null)
                        {
                            throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                        }
                    }
                    else
                    {
                        workItemsWaiting.WaitOne();
                    }
                }
            }
        }
    }

    public class LazyEx<T>
    {
        private readonly Func<Task<T>> instanceFactory;
        private TaskCompletionSource<bool> initTask;
        private int syncPoint;
        private T value;

        public LazyEx(Func<Task<T>> instanceFactory)
        {
            this.instanceFactory = instanceFactory;
            initTask = new TaskCompletionSource<bool>();
        }

        public LazyEx(T directValue)
        {
            initTask = null;
            value = directValue;
            Interlocked.Exchange(ref syncPoint, 1);
        }

        public T Value
        {
            get
            {
                EnsureLoaded();
                return value;
            }
        }

        public bool IsValueLoaded
        {
            get { return initTask == null; }
        }

        public Func<Task<T>> GetActivationFactory()
        {
            return instanceFactory;
        }

        public async Task EnsureLoadedAsync()
        {
            if (Interlocked.CompareExchange(ref syncPoint, 1, 0) == 0)
            {
                value = await instanceFactory().ConfigureAwait(false);
                initTask.TrySetResult(true);
                initTask = null;
                return;
            }

            var tcs = initTask;
            if (tcs != null)
            {
                await tcs.Task.ConfigureAwait(false);
            }
        }

        private void EnsureLoaded()
        {
            if (Interlocked.CompareExchange(ref syncPoint, 1, 0) == 0)
            {
                value = Task.Factory.StartNew(instanceFactory).Unwrap().Result;
                initTask.TrySetResult(true);
                initTask = null;
                return;
            }

            var tcs = initTask;
            if (tcs != null)
            {
                tcs.Task.Wait();
            }
        }
    }
}
