// Author: Prasanna V. Loganathar
// Created: 7:14 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.ObjectModel;
using RedScooby.Data.Models;
using RedScooby.ViewModels;
using RedScooby.ViewModels.Fragments;

namespace RedScooby.Components
{
    public static class ComponentsHelper
    {
        public static IDisposable AddUserSetupScope()
        {
            const string userSetupScopeName = "userSetup";

            var current = ViewModelLocator.Scope;
            if (current.ScopeName == userSetupScopeName)
                return ViewModelLocator.GetReversionDisposableForScope(current);

            var countryCollection = new ObservableCollection<CountryInfo>();
            return ViewModelLocator.BeginScope(c =>
            {
                c.ExportInstance((f, ctx) => new CountryInfoViewModel(
                    f.Locate<RegionManager>(),
                    countryCollection));
            }, userSetupScopeName);
        }
    }
}
