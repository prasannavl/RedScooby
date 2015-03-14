// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedScooby.Infrastructure.Framework.Commands
{
    public static class CommandFactory
    {
        public static RelayCommand Create(Action execute)
        {
            return new RelayCommand(execute, null);
        }

        public static RelayCommand Create<T>(Func<T> execute)
        {
            return new RelayCommand(() => execute(), null);
        }

        public static RelayCommand Create(Action execute, Func<bool> canExecute)
        {
            return new RelayCommand(execute, canExecute);
        }

        public static RelayCommand Create<T>(Func<T> execute, Func<bool> canExecute)
        {
            return new RelayCommand(() => execute(), canExecute);
        }

        public static AsyncRelayCommand CreateAsync(Func<Task> execute)
        {
            return new AsyncRelayCommand(execute, null);
        }

        public static AsyncRelayCommand CreateAsync(Func<Task> execute, Func<bool> canExecute)
        {
            return new AsyncRelayCommand(execute, canExecute);
        }

        public static AsyncRelayCommand CreateAsync<TResult>(Func<Task<TResult>> execute)
        {
            return new AsyncRelayCommand(execute, null);
        }

        public static AsyncRelayCommand CreateAsync<TResult>(Func<Task<TResult>> execute, Func<bool> canExecute)
        {
            return new AsyncRelayCommand(execute, canExecute);
        }

        public static AsyncRelayCommandCancellable CreateAsync(Func<CancellationToken, Task> execute)
        {
            return new AsyncRelayCommandCancellable(execute, null);
        }

        public static AsyncRelayCommandCancellable CreateAsync(Func<CancellationToken, Task> execute,
            Func<bool> canExecute)
        {
            return new AsyncRelayCommandCancellable(execute, canExecute);
        }

        public static AsyncRelayCommandCancellable CreateAsync<TResult>(Func<CancellationToken, Task<TResult>> execute)
        {
            return new AsyncRelayCommandCancellable(execute, null);
        }

        public static AsyncRelayCommandCancellable CreateAsync<TResult>(Func<CancellationToken, Task<TResult>> execute,
            Func<bool> canExecute)
        {
            return new AsyncRelayCommandCancellable(execute, canExecute);
        }

        public static AsyncRelayCommand<TParameter> CreateAsyncWithParameter<TParameter>(Func<TParameter, Task> execute)
        {
            return new AsyncRelayCommand<TParameter>(execute, null);
        }

        public static AsyncRelayCommand<TParameter> CreateAsyncWithParameter<TParameter>(Func<TParameter, Task> execute,
            Func<TParameter, bool> canExecute)
        {
            return new AsyncRelayCommand<TParameter>(execute, canExecute);
        }

        public static AsyncRelayCommand<TParameter> CreateAsyncWithParameter<TParameter, TResult>(
            Func<TParameter, Task<TResult>> execute)
        {
            return new AsyncRelayCommand<TParameter>(execute, null);
        }

        public static AsyncRelayCommand<TParameter> CreateAsyncWithParameter<TParameter, TResult>(
            Func<TParameter, Task<TResult>> execute, Func<TParameter, bool> canExecute)
        {
            return new AsyncRelayCommand<TParameter>(execute, canExecute);
        }

        public static AsyncRelayCommandCancellable<TParameter> CreateAsyncWithParameter<TParameter>(
            Func<TParameter, CancellationToken, Task> execute)
        {
            return new AsyncRelayCommandCancellable<TParameter>(execute, null);
        }

        public static AsyncRelayCommandCancellable<TParameter> CreateAsyncWithParameter<TParameter>(
            Func<TParameter, CancellationToken, Task> execute, Func<TParameter, bool> canExecute)
        {
            return new AsyncRelayCommandCancellable<TParameter>(execute, canExecute);
        }

        public static AsyncRelayCommandCancellable<TParameter> CreateAsyncWithParameter<TParameter, TResult>(
            Func<TParameter, CancellationToken, Task<TResult>> execute)
        {
            return new AsyncRelayCommandCancellable<TParameter>(execute, null);
        }

        public static AsyncRelayCommandCancellable<TParameter> CreateAsyncWithParameter<TParameter, TResult>(
            Func<TParameter, CancellationToken, Task<TResult>> execute, Func<TParameter, bool> canExecute)
        {
            return new AsyncRelayCommandCancellable<TParameter>(execute, canExecute);
        }

        public static RelayCommand<TParameter> CreateWithParameter<TParameter>(Action<TParameter> execute)
        {
            return new RelayCommand<TParameter>(execute, null);
        }

        public static RelayCommand<TParameter> CreateWithParameter<TParameter>(Action<TParameter> execute,
            Func<TParameter, bool> canExecute)
        {
            return new RelayCommand<TParameter>(execute, null);
        }
    }
}
