// Author: Prasanna V. Loganathar
// Created: 9:30 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using RedScooby.Api;
using RedScooby.Api.Endpoints;
using RedScooby.Components;
using RedScooby.Data.Models;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.Logging;
using RedScooby.Models;
using RedScooby.Utilities;
using RedScooby.Validation;
using RedScooby.ViewModels.Fragments;

namespace RedScooby.ViewModels.Setup
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly CountryInfoViewModel countryInfoViewModel;
        private readonly PhoneNumberFormatterViewModel phoneNumberFormatterViewModel;
        private readonly AppModel appModel;
        private readonly RegionManager regionManager;
        private string verificationCode;
        private string phoneVerificationToken;

        public LoginViewModel(AppModel appModel, RegionManager regionManager, CountryInfoViewModel countryInfoViewModel,
            PhoneNumberFormatterViewModel phoneNumberFormatterViewModel)
        {
            this.appModel = appModel;
            this.regionManager = regionManager;
            this.countryInfoViewModel = countryInfoViewModel;
            this.phoneNumberFormatterViewModel = phoneNumberFormatterViewModel;

            PopulateCountryInfoAsyncCommand = countryInfoViewModel.PopulateCountryInfoAsyncCommand;

            SelectCurrentRegionAsyncCommand =
                CommandFactory.CreateAsync(() =>
                {
                    return countryInfoViewModel.PopulateCountryInfoAsyncCommand.ExecuteAsync()
                        .ContinueWith(
                            t =>
                            {
                                if (SelectedCountryInfo != null)
                                    phoneNumberFormatterViewModel.Region = SelectedCountryInfo.Iso2LetterAlphaCode;
                            });
                });

            var map = new[]
            {
                new KeyValuePair<string, string>(
                    ExpressionHelpers.GetMemberName(() => countryInfoViewModel.CountryCollection),
                    ExpressionHelpers.GetMemberName(() => CountryCollection)),
                new KeyValuePair<string, string>(
                    ExpressionHelpers.GetMemberName(() => countryInfoViewModel.SelectedCountryInfo),
                    ExpressionHelpers.GetMemberName(() => SelectedCountryInfo))
            };

            MapNotificationsFrom(countryInfoViewModel, map, true);

            MapNotificationsFrom(phoneNumberFormatterViewModel,
                new KeyValuePair<string, string>(
                    ExpressionHelpers.GetMemberName(() => phoneNumberFormatterViewModel.PhoneNumber),
                    ExpressionHelpers.GetMemberName(() => PhoneNumber)), true);
        }

        public AsyncRelayCommand PopulateCountryInfoAsyncCommand { get; private set; }
        public AsyncRelayCommand SelectCurrentRegionAsyncCommand { get; private set; }

        public ObservableCollection<CountryInfo> CountryCollection
        {
            get { return countryInfoViewModel.CountryCollection; }
            set { countryInfoViewModel.CountryCollection = value; }
        }

        public CountryInfo SelectedCountryInfo
        {
            get { return countryInfoViewModel.SelectedCountryInfo; }
            set
            {
                countryInfoViewModel.SelectedCountryInfo = value;
                if (value != null)
                    phoneNumberFormatterViewModel.Region = value.Iso2LetterAlphaCode;
            }
        }

        public string PhoneNumber
        {
            get { return phoneNumberFormatterViewModel.PhoneNumber; }
            set { phoneNumberFormatterViewModel.PhoneNumber = value; }
        }

        public string VerificationCode
        {
            get { return verificationCode; }
            set { SetAndNotifyIfChanged(ref verificationCode, value); }
        }

        public void LoadFromUserModel(UserModel model)
        {
            PhoneNumber = model.PhoneNumber;
            if (model.HomeCountryId != null)
                SelectedCountryInfo = CountryCollection.First(x => x.Iso2LetterAlphaCode == model.HomeCountryId);
        }

        public async Task<PhoneVerificationResult> RequestPhoneVerificationAsync()
        {
            var phoneValidator = new PhoneNumberValidator(regionManager);
            var validity = phoneValidator.Validate(PhoneNumber);
            if (!validity.IsValid)
            {
                return new PhoneVerificationResult {Error = validity.Errors.First().ErrorMessage};
            }

            var tsk = SaveModelAsync();

            var res = await RedScoobyApi.Client.Account.RequestPhoneVerification(PhoneNumber,
                AccountEndpoint.PhoneVerificationType.Text);

            await tsk;

            if (res.Error == null)
            {
                phoneVerificationToken = res.Token;
                return new PhoneVerificationResult {Success = true};
            }
            else
            {
                phoneVerificationToken = null;
                return new PhoneVerificationResult {Error = res.Error};
            }
        }

        public Task SaveModelAsync()
        {
            var phNumber = PhoneNumber;
            if (phNumber != null)
            {
                var data = UserModel.CreateLoggedOutUser(phNumber);

                var selectedCountryInfo = SelectedCountryInfo;
                if (selectedCountryInfo != null)
                    data.HomeCountryId = selectedCountryInfo.Iso2LetterAlphaCode;

                appModel.User = data;
                appModel.LocalSettings.IsFirstRun = false;
                return appModel.SaveAllAsync();
            }

            return TaskCache.Completed;
        }

        public async Task<LoginResult> LoginAsync()
        {
            if (phoneVerificationToken == null)
            {
                Log.Error("Phone verification token empty.");
                return new LoginResult {Error = "Verification process has not been started."};
            }
            if (VerificationCode == null || VerificationCode.Length < 4)
            {
                return new LoginResult {Error = "Invalid verification code."};
            }

            var res = await RedScoobyApi.Client.Account.Login(PhoneNumber, phoneVerificationToken, VerificationCode);
            if (res.Error == null)
            {
                //TODO: Add security data
                appModel.User = res.User;
                await appModel.SaveUserAsync();

                return new LoginResult {Success = true};
            }

            return new LoginResult {Error = res.Error};
        }

        public ValidationResult ValidatePhoneNumber()
        {
            var validator = new PhoneNumberValidator(regionManager);
            return validator.Validate(PhoneNumber);
        }

        public class LoginResult
        {
            public bool Success;
            public string Error;
        }

        public class PhoneVerificationResult
        {
            public bool Success;
            public string Error;
        }
    }
}
