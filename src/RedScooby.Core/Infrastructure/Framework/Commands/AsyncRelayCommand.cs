// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Scaffold;

namespace RedScooby.Infrastructure.Framework.Commands
{
    public class AsyncRelayCommand : IAsyncCommand
    {
        private readonly Func<bool> canExecute;
        private readonly Func<Task> executeAsync;
        private NotifyingTaskWrapper taskWrapper;

        internal AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute");

            executeAsync = execute;
            this.canExecute = canExecute;
        }

        public Task ExecuteAsync(bool handleError = true)
        {
            taskWrapper = new NotifyingTaskWrapper(executeAsync());
            RaiseCanExecuteChanged();
            return taskWrapper.Task.ContinueOnCurrentContextWith(t =>
            {
                RaiseCanExecuteChanged();
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

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler.Invoke(this, EventArgs.Empty);
        }
    }

    public class AsyncRelayCommand<TParameter> : IAsyncCommand<TParameter>
    {
        private readonly Func<TParameter, bool> canExecute;
        private readonly Func<TParameter, Task> executeAsync;
        private NotifyingTaskWrapper taskWrapper;

        internal AsyncRelayCommand(Func<TParameter, Task> execute, Func<TParameter, bool> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute");

            executeAsync = execute;
            this.canExecute = canExecute;
        }

        public Task ExecuteAsync(TParameter parameter, bool handleError = true)
        {
            taskWrapper = new NotifyingTaskWrapper(executeAsync(parameter));
            RaiseCanExecuteChanged();
            return taskWrapper.Task.ContinueOnCurrentContextWith(t =>
            {
                RaiseCanExecuteChanged();
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

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler.Invoke(this, EventArgs.Empty);
        }
    }
}
