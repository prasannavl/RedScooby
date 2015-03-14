// Author: Prasanna V. Loganathar
// Created: 7:14 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhoneNumbers;
using RedScooby.Data;
using RedScooby.Data.Models;

namespace RedScooby.Components
{
    public class RegionManager : IDisposable
    {
        private readonly CountryInfoProvider countryInfoProvider;
        private readonly Func<PhoneNumberUtil> phoneNumberUtilFunc;
        private Task<IDictionary<string, CountryInfo>> countryInfoTask;
        private PhoneNumberUtil phoneNumberUtilInstance;

        public RegionManager(CountryInfoProvider countryInfoProvider, Func<PhoneNumberUtil> phoneNumberUtilFunc)
        {
            this.countryInfoProvider = countryInfoProvider;
            this.phoneNumberUtilFunc = phoneNumberUtilFunc;
        }

        public void Dispose()
        {
            ReleasePhoneNumberUtil();
        }

        public void CleanupCountryInfo()
        {
            countryInfoTask = null;
        }

        public Task<IDictionary<string, CountryInfo>> GetCountryInfoAsync()
        {
            return countryInfoTask ?? (countryInfoTask = countryInfoProvider.GetCountryInfo());
        }

        public PhoneNumberUtil GetPhoneNumberUtil()
        {
            return phoneNumberUtilInstance ?? (phoneNumberUtilInstance = phoneNumberUtilFunc());
        }

        public Task<IDictionary<string, CountryInfo>> PopulateCountriesAsync(bool forceRepopulate = false)
        {
            if (forceRepopulate || countryInfoTask == null)
                countryInfoTask = countryInfoProvider.GetCountryInfo();

            return countryInfoTask;
        }

        public void ReleasePhoneNumberUtil()
        {
            phoneNumberUtilInstance = null;
        }
    }
}
