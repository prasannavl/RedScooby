// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading;
using System.Windows.Input;

namespace RedScooby.Infrastructure.Framework.Commands
{
    public class SingleShotCancelCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly CancellationTokenSource tokenSource;
        private bool? canExecuteOverride;

        public SingleShotCancelCommand()
        {
            tokenSource = new CancellationTokenSource();
        }

        public SingleShotCancelCommand(Func<bool> canExecute) : this()
        {
            this.canExecute = canExecute;
        }

        public CancellationToken Token
        {
            get { return tokenSource.Token; }
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (canExecuteOverride.HasValue) return canExecuteOverride.Value;
            return !tokenSource.IsCancellationRequested && (canExecute == null || canExecute());
        }

        public void Execute(object parameter)
        {
            tokenSource.Cancel();
            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler.Invoke(this, EventArgs.Empty);
        }

        public void SetCanExecuteOverride(bool? value)
        {
            canExecuteOverride = value;
        }
    }
}
