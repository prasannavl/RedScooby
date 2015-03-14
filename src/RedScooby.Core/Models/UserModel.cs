// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.IO;
using System.Runtime.Serialization;
using RedScooby.Data.Core;

namespace RedScooby.Models
{
    public sealed class UserModel : PersistentDispatchingObject
    {
        private string homeCountryId;
        private DateTimeOffset dateOfBirth;
        private string email;
        private Gender gender;
        private int id;
        private string name;
        private string phoneNumber;
        private string profileImageUrl;
        private UserSettingsModel settings;
        private UserSessionModel session;
        private Stream profileImage;
        private UserFocus userFocus;

        public UserModel()
        {
            settings = new UserSettingsModel();
            session = new UserSessionModel();
        }

        public UserModel(StreamingContext context) : base(null)
        {
            settings = new UserSettingsModel(context);
            session = new UserSessionModel(context);
        }

        public int Id
        {
            get { return id; }
            set { SetAndNotifyIfChanged(ref id, value); }
        }

        public string Name
        {
            get { return name; }
            set { SetAndNotifyIfChanged(ref name, value); }
        }

        public string ProfileImageUrl
        {
            get { return profileImageUrl; }
            set { SetAndNotifyIfChanged(ref profileImageUrl, value); }
        }

        public string HomeCountryId
        {
            get { return homeCountryId; }
            set { SetAndNotifyIfChanged(ref homeCountryId, value); }
        }

        public DateTimeOffset DateOfBirth
        {
            get { return dateOfBirth; }
            set
            {
                SetAndNotifyIfChanged(ref dateOfBirth,
                    value, () => NotifyPropertyChanged(() => Age));
            }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { SetAndNotifyIfChanged(ref phoneNumber, value); }
        }

        public Gender Gender
        {
            get { return gender; }
            set { SetAndNotifyIfChanged(ref gender, value); }
        }

        public string Email
        {
            get { return email; }
            set { SetAndNotifyIfChanged(ref email, value); }
        }

        public int Age
        {
            get
            {
                var age = DateTimeOffset.Now.Year - DateOfBirth.Year;
                if (DateOfBirth > DateTimeOffset.Now.AddYears(-age))
                    age--;
                return age;
            }
        }

        public Stream ProfileImage
        {
            get { return profileImage; }
            set { SetAndNotifyIfChanged(ref profileImage, value); }
        }

        public UserSettingsModel Settings
        {
            get { return settings; }
            set { SetAndNotifyIfChanged(ref settings, value); }
        }

        public UserSessionModel Session
        {
            get { return session; }
            set { SetAndNotifyIfChanged(ref session, value); }
        }

        public UserFocus UserFocus
        {
            get { return userFocus; }
            set { SetAndNotifyIfChanged(ref userFocus, value); }
        }

        public static UserModel CreateLoggedOutUser(string phoneNumber = null, string countryId = null)
        {
            var user = new UserModel {Id = -1, PhoneNumber = phoneNumber, HomeCountryId = countryId};
            return user;
        }
    }

    public sealed class UserSessionModel : PersistentDispatchingObject
    {
        private bool isAuthenticationValid;
        public UserSessionModel() { }
        public UserSessionModel(StreamingContext context) : base(null) { }

        public bool IsAuthenticationValid
        {
            get { return isAuthenticationValid; }
            set { SetAndNotifyIfChanged(ref isAuthenticationValid, value); }
        }
    }

    public sealed class UserSettingsModel : PersistentDispatchingObject
    {
        private string disasterPinCode;
        private string pinCode;
        private string threeDigitDisasterPinCode;
        private string threeDigitPinCode;
        public UserSettingsModel() { }
        public UserSettingsModel(StreamingContext context) : base(null) { }

        public string PinCode
        {
            get { return pinCode; }
            set { SetAndNotifyIfChanged(ref pinCode, value, () => { ThreeDigitPinCode = GetThreeDigitPin(pinCode); }); }
        }

        public string ThreeDigitPinCode
        {
            get { return threeDigitPinCode; }
            private set { SetAndNotifyIfChanged(ref threeDigitPinCode, value); }
        }

        public string ThreeDigitDisasterPinCode
        {
            get { return threeDigitDisasterPinCode; }
            private set { SetAndNotifyIfChanged(ref threeDigitDisasterPinCode, value); }
        }

        public string DisasterPinCode
        {
            get { return disasterPinCode; }
            set
            {
                SetAndNotifyIfChanged(ref disasterPinCode,
                    value, () => { ThreeDigitDisasterPinCode = GetThreeDigitPin(disasterPinCode); });
            }
        }

        private string GetThreeDigitPin(string fourDigitPin)
        {
            var codeString = fourDigitPin;
            var len = codeString.Length;
            if (len == 4)
                return codeString.Substring(0, len - 1);
            return null;
        }
    }

    [Flags]
    public enum UserFocus : byte
    {
        General,
        Privilaged,
        PrivateSecurity,
        PublicSecurity,
        Medicine,
        Media
    }

    public enum Gender : byte
    {
        Male,
        Female,
    }
}
