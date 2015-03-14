// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using RedScooby.Utilities;

namespace RedScooby.Infrastructure.Scaffold
{
    public sealed class NotifyingTaskWrapper : INotifyPropertyChanged
    {
        public NotifyingTaskWrapper(Task task)
        {
            Task = task;
            var _ = GetWrappedTask();
        }

        public Task Task { get; private set; }

        public TaskStatus Status
        {
            get { return Task.Status; }
        }

        public bool IsCompleted
        {
            get { return Task.IsCompleted; }
        }

        public bool HasNotCompleted
        {
            get { return !Task.IsCompleted; }
        }

        public bool HasSuccessfullyCompleted
        {
            get
            {
                return Task.Status ==
                       TaskStatus.RanToCompletion;
            }
        }

        public bool IsCanceled
        {
            get { return Task.IsCanceled; }
        }

        public bool IsFaulted
        {
            get { return Task.IsFaulted; }
        }

        public AggregateException Exception
        {
            get { return Task.Exception; }
        }

        public Exception InnerException
        {
            get { return Exception != null ? Exception.InnerException : null; }
        }

        public string ErrorMessage
        {
            get { return InnerException != null ? InnerException.Message : null; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Task GetWrappedTask()
        {
            return Task.Status != TaskStatus.Created
                ? TaskCache.Completed
                : Task.ContinueWith(NotifyPropertyChanges, CancellationToken.None,
                    TaskContinuationOptions.DenyChildAttach,
                    SynchronizationContext.Current == null
                        ? TaskScheduler.Current
                        : TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void NotifyPropertyChanges(Task task)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("Status"));
                handler(this, new PropertyChangedEventArgs("IsCompleted"));
                handler(this, new PropertyChangedEventArgs("HasNotCompleted"));

                if (task.IsCanceled)
                {
                    handler(this, new PropertyChangedEventArgs("IsCanceled"));
                }
                else if (task.IsFaulted)
                {
                    handler(this, new PropertyChangedEventArgs("IsFaulted"));
                    handler(this, new PropertyChangedEventArgs("Exception"));
                    handler(this,
                        new PropertyChangedEventArgs("InnerException"));
                    handler(this, new PropertyChangedEventArgs("ErrorMessage"));
                }
                else
                {
                    handler(this,
                        new PropertyChangedEventArgs("HasSuccessfullyCompleted"));
                }
            }
        }
    }

    public sealed class NotifyingTaskWrapper<TResult> : INotifyPropertyChanged
    {
        public NotifyingTaskWrapper(Task<TResult> task)
        {
            Task = task;
            var _ = GetWrappedTask();
        }

        public Task<TResult> Task { get; private set; }

        public TResult Result
        {
            get
            {
                return (Task.Status == TaskStatus.RanToCompletion)
                    ? Task.Result
                    : default(TResult);
            }
        }

        public TaskStatus Status
        {
            get { return Task.Status; }
        }

        public bool IsCompleted
        {
            get { return Task.IsCompleted; }
        }

        public bool HasNotCompleted
        {
            get { return !Task.IsCompleted; }
        }

        public bool HasSuccessfullyCompleted
        {
            get
            {
                return Task.Status ==
                       TaskStatus.RanToCompletion;
            }
        }

        public bool IsCanceled
        {
            get { return Task.IsCanceled; }
        }

        public bool IsFaulted
        {
            get { return Task.IsFaulted; }
        }

        public AggregateException Exception
        {
            get { return Task.Exception; }
        }

        public Exception InnerException
        {
            get { return Exception != null ? Exception.InnerException : null; }
        }

        public string ErrorMessage
        {
            get { return InnerException != null ? InnerException.Message : null; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Task GetWrappedTask()
        {
            return Task.Status != TaskStatus.Created
                ? TaskCache.Completed
                : Task.ContinueWith(NotifyPropertyChanges, CancellationToken.None,
                    TaskContinuationOptions.DenyChildAttach,
                    SynchronizationContext.Current == null
                        ? TaskScheduler.Current
                        : TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void NotifyPropertyChanges(Task task)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("Status"));
                handler(this, new PropertyChangedEventArgs("IsCompleted"));
                handler(this, new PropertyChangedEventArgs("HasNotCompleted"));

                if (task.IsCanceled)
                {
                    handler(this, new PropertyChangedEventArgs("IsCanceled"));
                }
                else if (task.IsFaulted)
                {
                    handler(this, new PropertyChangedEventArgs("IsFaulted"));
                    handler(this, new PropertyChangedEventArgs("Exception"));
                    handler(this,
                        new PropertyChangedEventArgs("InnerException"));
                    handler(this, new PropertyChangedEventArgs("ErrorMessage"));
                }
                else
                {
                    handler(this,
                        new PropertyChangedEventArgs("HasSuccessfullyCompleted"));
                    handler(this, new PropertyChangedEventArgs("Result"));
                }
            }
        }
    }
}
