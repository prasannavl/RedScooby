// Author: Prasanna V. Loganathar
// Created: 9:23 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Windows.UI.Xaml.Controls;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.ViewModels.Circles;
using RedScooby.Views.Components;

namespace RedScooby.Views.Circles
{
    public class CirclesViewBase : UserControlView<CirclesViewModel> { }

    public sealed partial class CirclesView
    {
        public CirclesView()
        {
            InitializeComponent();
        }

        public AppBar GetAppBar()
        {
            var appBar = new CommandBar();
            appBar.PrimaryCommands.Add(new AppBarButton
            {
                Icon = new SymbolIcon(Symbol.Add),
                Label = "add circle",
                Command = CommandFactory.CreateAsync(WindowHelpers.ShowOperationNotAvailableOnBetaDialogAsync)
            });

            appBar.PrimaryCommands.Add(new AppBarButton
            {
                Icon = new SymbolIcon(Symbol.AddFriend),
                Label = "quick share",
                Command = CommandFactory.CreateAsync(WindowHelpers.ShowOperationNotAvailableOnBetaDialogAsync)
            });

            return appBar;
        }
    }
}
