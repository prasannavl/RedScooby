// Author: Prasanna V. Loganathar
// Created: 9:30 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using RedScooby.Api;
using RedScooby.Components;
using RedScooby.Data.Models;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.Models;
using RedScooby.Validation;
using RedScooby.ViewModels.Fragments;

namespace RedScooby.ViewModels.Setup
{
    public class RegistrationViewModel : ViewModelBase
    {
        private readonly CountryInfoViewModel countryInfoViewModel;
        private readonly PhoneNumberFormatterViewModel phoneNumberFormatterViewModel;
        private readonly AppModel appModel;
        private readonly RegionManager regionManager;
        private bool isSecurityFocus;
        private UserModel user;
        private bool isMedicineFocus;
        private bool isMediaFocus;
        private string modelText;

        public RegistrationViewModel(AppModel appModel, RegionManager regionManager,
            CountryInfoViewModel countryInfoViewModel,
            PhoneNumberFormatterViewModel phoneNumberFormatterViewModel)
        {
            user = new UserModel();

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

        public UserModel User
        {
            get { return user; }
            private set { SetAndNotifyIfChanged(ref user, value); }
        }

        public string ModalText
        {
            get { return modelText; }
            private set { SetAndNotifyIfChanged(ref modelText, value); }
        }

        public bool IsSecurityFocus
        {
            get { return isSecurityFocus; }
            set { SetAndNotifyIfChanged(ref isSecurityFocus, value); }
        }

        public bool IsMedicineFocus
        {
            get { return isMedicineFocus; }
            set { SetAndNotifyIfChanged(ref isMedicineFocus, value); }
        }

        public bool IsMediaFocus
        {
            get { return isMediaFocus; }
            set { SetAndNotifyIfChanged(ref isMediaFocus, value); }
        }

        public ValidationResult Validate()
        {
            var validator = new UserValidator();
            var userResult = validator.Validate(User);

            var phoneNumberValidator = new PhoneNumberValidator(regionManager);
            var phResult = phoneNumberValidator.Validate(PhoneNumber);

            var result = new ValidationResult(userResult.Errors.Concat(phResult.Errors));
            return result;
        }

        public void FillUserModel()
        {
            User.PhoneNumber = PhoneNumber;
            User.HomeCountryId = SelectedCountryInfo == null ? null : SelectedCountryInfo.Iso2LetterAlphaCode;
            User.UserFocus = UserFocus.General;
            if (IsSecurityFocus)
            {
                User.UserFocus |= UserFocus.PrivateSecurity;
            }
            if (IsMedicineFocus)
            {
                User.UserFocus |= UserFocus.Medicine;
            }
            if (IsMedicineFocus)
            {
                User.UserFocus |= UserFocus.Media;
            }
        }

        public async Task SaveModelAsync()
        {
            FillUserModel();
            appModel.User = User;
            appModel.LocalSettings.IsFirstRun = false;
            await appModel.SaveAllAsync();
        }

        public async Task SaveIfRequired()
        {
            if (!string.IsNullOrEmpty(User.Name) || !string.IsNullOrEmpty(User.Email))
            {
                await SaveModelAsync();
            }
        }

        public async Task<RegistrationResult> RegisterAsync()
        {
            FillUserModel();
            var res = await RedScoobyApi.Client.Account.RegisterUser(User);
            if (res.Error == null)
            {
                var regUser = User;
                User = new UserModel();
                return new RegistrationResult {Success = true, RegisteredUser = regUser};
            }

            return new RegistrationResult {Error = res.Error};
        }

        public void LoadFromUserModel(UserModel userModel)
        {
            User = userModel;
            PhoneNumber = User.PhoneNumber;

            if (User.HomeCountryId != null)
                SelectedCountryInfo =
                    CountryCollection.First(x => x.Iso2LetterAlphaCode == User.HomeCountryId);

            IsSecurityFocus = User.UserFocus.HasFlag(UserFocus.PrivateSecurity) ||
                              User.UserFocus.HasFlag(UserFocus.PublicSecurity);
            IsMedicineFocus = User.UserFocus.HasFlag(UserFocus.Medicine);
            IsMediaFocus = User.UserFocus.HasFlag(UserFocus.Media);
        }

        public ValidationResult ValidateBasicInfo()
        {
            var validator = new UserValidator();
            return validator.Validate(User,
                ruleSet: UserValidator.UserValidationRuleSet.RegistrationBasic.ToString());
        }

        public ValidationResult ValidatePhoneNumber()
        {
            var validator = new PhoneNumberValidator(regionManager);
            return validator.Validate(PhoneNumber);
        }

        public ValidationResult ValidatePinCodes()
        {
            var validator = new UserValidator();
            return validator.Validate(User,
                ruleSet: UserValidator.UserValidationRuleSet.SecurityPinCodes.ToString());
        }

        protected override async Task OnSuspendAsync()
        {
            await base.OnSuspendAsync();
            await SaveIfRequired();
        }

        public class RegistrationResult
        {
            public bool Success;
            public UserModel RegisteredUser;
            public string Error;
        }
    }
}
