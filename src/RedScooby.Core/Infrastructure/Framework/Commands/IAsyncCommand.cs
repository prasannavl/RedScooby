// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RedScooby.Infrastructure.Framework.Commands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(bool handleError = true);
    }

    public interface IAsyncCommand<TParameter> : ICommand
    {
        Task ExecuteAsync(TParameter parameter, bool handleError = true);
    }

    public interface IWithCancelCommand
    {
        ICommand CancelCommand { get; }
    }

    public interface IAsyncCommandCancellable : IAsyncCommand, IWithCancelCommand, INotifyPropertyChanged { }

    public interface IAsyncCommandCancellable<TParameter> : IAsyncCommand<TParameter>, IWithCancelCommand,
        INotifyPropertyChanged { }
}
