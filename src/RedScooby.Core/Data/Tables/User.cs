// Author: Prasanna V. Loganathar
// Created: 4:47 PM 04-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using RedScooby.Models;

namespace RedScooby.Data.Tables
{
    public sealed class User : DbObjectWithId<UserModel>
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string HomeCountryId { get; set; }
        public string ProfileImageUrl { get; set; }
        public UserFocus UserFocus { get; set; }

        public override void PopulateModel(UserModel model)
        {
            base.PopulateModel(model);

            model.Id = UserId;
            model.Name = Name;
            model.Email = Email;
            model.Gender = Gender;
            model.DateOfBirth = DateOfBirth;
            model.PhoneNumber = PhoneNumber;
            model.ProfileImageUrl = ProfileImageUrl;
            model.HomeCountryId = HomeCountryId;
            model.UserFocus = UserFocus;
        }

        public override void PopulateFromModel(UserModel model)
        {
            base.PopulateFromModel(model);

            UserId = model.Id;
            Name = model.Name;
            Email = model.Email;
            Gender = model.Gender;
            DateOfBirth = model.DateOfBirth;
            PhoneNumber = model.PhoneNumber;
            ProfileImageUrl = model.ProfileImageUrl;
            HomeCountryId = HomeCountryId;
            UserFocus = model.UserFocus;
        }
    }


    public sealed class UserSession : DbObjectWithId<UserSessionModel>
    {
        public bool IsAuthenticationValid { get; set; }

        public override void PopulateModel(UserSessionModel model)
        {
            base.PopulateModel(model);
            model.IsAuthenticationValid = IsAuthenticationValid;
        }

        public override void PopulateFromModel(UserSessionModel model)
        {
            base.PopulateFromModel(model);
            IsAuthenticationValid = model.IsAuthenticationValid;
        }
    }

    internal sealed class UserSettings : DbObjectWithId<UserSettingsModel>
    {
        public string PinCode { get; set; }
        public string DisasterPinCode { get; set; }

        public override void PopulateModel(UserSettingsModel model)
        {
            base.PopulateModel(model);
            model.DisasterPinCode = DisasterPinCode;
            model.PinCode = PinCode;
        }

        public override void PopulateFromModel(UserSettingsModel model)
        {
            base.PopulateFromModel(model);
            DisasterPinCode = model.DisasterPinCode;
            PinCode = model.PinCode;
        }
    }
}
