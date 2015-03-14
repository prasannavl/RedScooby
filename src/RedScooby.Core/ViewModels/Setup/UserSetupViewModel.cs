// Author: Prasanna V. Loganathar
// Created: 9:30 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Threading.Tasks;
using RedScooby.Components;
using RedScooby.Infrastructure.Composition;
using RedScooby.Logging;
using RedScooby.Models;

namespace RedScooby.ViewModels.Setup
{
    public class UserSetupViewModel : ViewModelBase
    {
        private static ILogger Log = Logging.Log.ForContext<UserSetupViewModel>();
        private readonly AppModel appModel;
        private readonly RegionManager regionManager;
        private bool isInitialized;

        public UserSetupViewModel(AppModel appModel, RegionManager regionManager,
            RegistrationViewModel registrationViewModel,
            LoginViewModel loginViewModel)
        {
            this.appModel = appModel;
            this.regionManager = regionManager;

            RegistrationViewModel = registrationViewModel;
            LoginViewModel = loginViewModel;

            AddDisposable(registrationViewModel);
            AddDisposable(loginViewModel);
        }

        public RegistrationViewModel RegistrationViewModel { get; private set; }
        public LoginViewModel LoginViewModel { get; private set; }

        public void Cleanup()
        {
            isInitialized = false;
            regionManager.ReleasePhoneNumberUtil();
            regionManager.CleanupCountryInfo();
        }

        public async Task InitializeAsync()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                regionManager.GetPhoneNumberUtil();
                await regionManager.PopulateCountriesAsync().ConfigureAwait(false);
                await RegistrationViewModel.PopulateCountryInfoAsyncCommand.ExecuteAsync().ConfigureAwait(false);
                await RegistrationViewModel.SelectCurrentRegionAsyncCommand.ExecuteAsync().ConfigureAwait(false);
                await LoginViewModel.PopulateCountryInfoAsyncCommand.ExecuteAsync().ConfigureAwait(false);
                await LoginViewModel.SelectCurrentRegionAsyncCommand.ExecuteAsync().ConfigureAwait(false);
            }
        }

        public void Load()
        {
            var user = appModel.User;
            if (user.Id == 0)
            {
                Log.Trace(f => f("Loading registration data from {0}", user));
                RegistrationViewModel.LoadFromUserModel(user);
            }
            else
            {
                Log.Trace(f => f("Loading login data from {0}", user));
                LoginViewModel.LoadFromUserModel(user);
            }
        }
    }
}
