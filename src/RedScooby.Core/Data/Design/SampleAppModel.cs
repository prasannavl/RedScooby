// Author: Prasanna V. Loganathar
// Created: 9:41 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using RedScooby.Models;

namespace RedScooby.Data.Design
{
    public sealed class SampleAppModel
    {
        public LocalSettingsModel GetSettingsModel()
        {
            var settings = new LocalSettingsModel
            {
                AutoVoiceOnDistress = false,
                IsDemoModeActive = false,
                IsDeveloperModeActive = true,
                IsFirstRun = false
            };

            return settings;
        }

        public UserModel GetUserModel()
        {
            var user = new UserModel
            {
                Name = "RedScooby Magical User",
                DateOfBirth = new DateTime(1980, 01, 01),
                Email = "magic@redscooby.com",
                Id = 21,
                PhoneNumber = "+91-1234567890",
                HomeCountryId = "IN",
            };

            user.Settings.DisasterPinCode = "1111";
            user.Settings.PinCode = "0000";
            user.Session.IsAuthenticationValid = false;

            return user;
        }
    }
}
