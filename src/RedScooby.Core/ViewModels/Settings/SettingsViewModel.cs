// Author: Prasanna V. Loganathar
// Created: 9:28 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Helpers;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.Models;

namespace RedScooby.ViewModels.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly AppModel model;

        public SettingsViewModel(AppModel model)
        {
            this.model = model;
            ResetUserCommand = CommandFactory.Create(() =>
            {
                var m = Model;
                var user = User;

                var newUser = UserModel.CreateLoggedOutUser(user.PhoneNumber, user.HomeCountryId);
                var newSettings = new LocalSettingsModel {IsFirstRun = false};

                m.User = newUser;
                m.LocalSettings = newSettings;

                App.Current.ApplicationExitDelegate.InvokeIfNotNullWith(true, true);
            });
        }

        public RelayCommand ResetUserCommand { get; private set; }

        public AppModel Model
        {
            get { return model; }
        }

        public UserModel User
        {
            get { return Model.User; }
        }

        public LocalSettingsModel LocalSettings
        {
            get { return Model.LocalSettings; }
        }
    }
}
