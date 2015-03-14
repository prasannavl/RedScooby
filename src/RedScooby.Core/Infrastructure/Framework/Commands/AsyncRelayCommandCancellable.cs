// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Scaffold;

namespace RedScooby.Infrastructure.Framework.Commands
{
    public class AsyncRelayCommandCancellable : IAsyncCommandCancellable
    {
        private readonly Func<bool> canExecute;
        private readonly Func<CancellationToken, Task> executeAsync;
        private SingleShotCancelCommand cancelCommand;
        private NotifyingTaskWrapper taskWrapper;

        internal AsyncRelayCommandCancellable(Func<CancellationToken, Task> execute, Func<bool> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute");

            executeAsync = execute;
            this.canExecute = canExecute;
            cancelCommand = CreateCancelCommand();
        }

        public Task ExecuteAsync(bool handleError = true)
        {
            taskWrapper = new NotifyingTaskWrapper(executeAsync(cancelCommand.Token));
            RaiseCanExecuteChanged();
            cancelCommand.SetCanExecuteOverride(true);
            cancelCommand.RaiseCanExecuteChanged();
            return taskWrapper.Task.ContinueOnCurrentContextWith(t =>
            {
                try
                {
                    if (t.IsCanceled)
                    {
                        CancelCommand = CreateCancelCommand();
                    }
                    else if (t.Status == TaskStatus.RanToCompletion)
                    {
                        cancelCommand.SetCanExecuteOverride(false);
                        cancelCommand.RaiseCanExecuteChanged();
                    }
                }
                finally
                {
                    RaiseCanExecuteChanged();
                }

                AsyncCommandResultDispatcher.DispatchResult(t, handleError);
            });
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return (taskWrapper == null || taskWrapper.IsCompleted) && (canExecute == null || canExecute());
        }

        public void Execute(object parameter)
        {
            var _ = ExecuteAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CancelCommand
        {
            get { return cancelCommand; }
            private set
            {
                cancelCommand = (SingleShotCancelCommand) value;
                OnPropertyChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal static SingleShotCancelCommand CreateCancelCommand()
        {
            var newCancelCommand = new SingleShotCancelCommand();
            newCancelCommand.SetCanExecuteOverride(false);
            return newCancelCommand;
        }
    }

    public class AsyncRelayCommandCancellable<TParameter> : IAsyncCommandCancellable<TParameter>
    {
        private readonly Func<TParameter, bool> canExecute;
        private readonly Func<TParameter, CancellationToken, Task> executeAsync;
        private SingleShotCancelCommand cancelCommand;
        private NotifyingTaskWrapper taskWrapper;

        internal AsyncRelayCommandCancellable(Func<TParameter, CancellationToken, Task> execute,
            Func<TParameter, bool> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute");

            executeAsync = execute;
            this.canExecute = canExecute;
            cancelCommand = AsyncRelayCommandCancellable.CreateCancelCommand();
        }

        public Task ExecuteAsync(TParameter parameter, bool handleError = true)
        {
            taskWrapper = new NotifyingTaskWrapper(executeAsync(parameter, cancelCommand.Token));
            RaiseCanExecuteChanged();
            cancelCommand.SetCanExecuteOverride(true);
            cancelCommand.RaiseCanExecuteChanged();
            return taskWrapper.Task.ContinueOnCurrentContextWith(t =>
            {
                try
                {
                    if (t.IsCanceled)
                    {
                        CancelCommand = AsyncRelayCommandCancellable.CreateCancelCommand();
                    }
                    else if (t.Status == TaskStatus.RanToCompletion)
                    {
                        cancelCommand.SetCanExecuteOverride(false);
                        cancelCommand.RaiseCanExecuteChanged();
                    }
                }
                finally
                {
                    RaiseCanExecuteChanged();
                }

                AsyncCommandResultDispatcher.DispatchResult(t, handleError);
            });
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return (taskWrapper == null || taskWrapper.IsCompleted) &&
                   (canExecute == null || canExecute((TParameter) parameter));
        }

        public void Execute(object parameter)
        {
            var _ = ExecuteAsync((TParameter) parameter);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CancelCommand
        {
            get { return cancelCommand; }
            private set
            {
                cancelCommand = (SingleShotCancelCommand) value;
                OnPropertyChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
