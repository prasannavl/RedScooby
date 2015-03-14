// Author: Prasanna V. Loganathar
// Created: 9:29 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using RedScooby.Components;
using RedScooby.Data.Models;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.Utilities;

namespace RedScooby.ViewModels.Fragments
{
    public class CountryInfoViewModel : ViewModelBase
    {
        public AsyncRelayCommand ForcePopulateCountryInfoAsyncCommand;
        public AsyncRelayCommand PopulateCountryInfoAsyncCommand;
        public AsyncRelayCommand SelectCurrentRegionAsyncCommand;
        private readonly RegionManager regionManager;
        private ObservableCollection<CountryInfo> countryCollection;
        private CountryInfo selectedCountryInfo;

        public CountryInfoViewModel(RegionManager regionManager,
            ObservableCollection<CountryInfo> sharedCountryInfoCollection = null)
        {
            this.regionManager = regionManager;
            PopulateCountryInfoAsyncCommand = CommandFactory.CreateAsync(() => PopulateCountryInfo());
            ForcePopulateCountryInfoAsyncCommand = CommandFactory.CreateAsync(() => PopulateCountryInfo(true));

            SelectCurrentRegionAsyncCommand = CommandFactory.CreateAsync(
                () =>
                {
                    CountryInfo currentInfo;
                    return regionManager.GetCountryInfoAsync().ContinueWith(t =>
                    {
                        if (t.Result.TryGetValue(RegionInfo.CurrentRegion.TwoLetterISORegionName, out currentInfo))
                        {
                            SelectedCountryInfo = currentInfo;
                        }
                    });
                });

            if (sharedCountryInfoCollection != null)
            {
                countryCollection = sharedCountryInfoCollection;
            }
            else
            {
                PopulateCountryInfo();
            }
        }

        public ObservableCollection<CountryInfo> CountryCollection
        {
            get { return countryCollection; }
            set { SetAndNotifyIfChanged(ref countryCollection, value); }
        }

        public CountryInfo SelectedCountryInfo
        {
            get { return selectedCountryInfo; }
            set
            {
                if (value != null)
                {
                    SetAndNotifyIfChanged(ref selectedCountryInfo, value);
                }
            }
        }

        private Task PopulateCountryInfo(bool forceRepopulate = false)
        {
            if (forceRepopulate || countryCollection == null || countryCollection.Count == 0)
            {
                return regionManager.GetCountryInfoAsync().ContinueWith(
                    t => { CountryCollection = new ObservableCollection<CountryInfo>(t.Result.Values); });
            }

            return TaskCache.Completed;
        }
    }
}
