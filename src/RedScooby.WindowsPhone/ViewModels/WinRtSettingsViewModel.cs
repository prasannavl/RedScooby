// Author: Prasanna V. Loganathar
// Created: 3:19 PM 23-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Helpers;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.Models;
using RedScooby.ViewModels.Settings;
using RedScooby.Views.Settings;

namespace RedScooby.ViewModels
{
    public class WinRtSettingsViewModel : ViewModelBase
    {
        private readonly SettingsViewModel coreViewModelViewModel;

        public WinRtSettingsViewModel(AppModel model, SettingsViewModel coreViewModelViewModel)
        {
            this.coreViewModelViewModel = coreViewModelViewModel;
            NavigateToAdvancedOptionsCommands =
                CommandFactory.CreateAsync(
                    async () => { await WindowHelpers.NavigateAsync(typeof (AdvancedOptionsView)); });
        }

        public AsyncRelayCommand NavigateToAdvancedOptionsCommands { get; private set; }

        public SettingsViewModel CoreViewModel
        {
            get { return coreViewModelViewModel; }
        }
    }
}
