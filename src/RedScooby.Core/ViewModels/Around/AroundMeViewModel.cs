// Author: Prasanna V. Loganathar
// Created: 9:29 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using RedScooby.Common.Resources;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Location;

namespace RedScooby.ViewModels.Around
{
    public class AroundMeViewModel : ViewModelBase
    {
        public static TimeSpan Recency = TimeSpan.FromMinutes(2);
        private readonly LocationManager locationManager;
        private GeoPosition lastKnownLocation;
        private Address lastKnownAddress;
        private IDisposable listener;

        public AroundMeViewModel(LocationManager locationManager)
        {
            this.locationManager = locationManager;

            LastKnownLocation = locationManager.LastKnownPosition;
            LastKnownAddress = locationManager.LastKnownAddress;

            AddDisposable(this.locationManager.PositionChanges.Subscribe(x =>
            {
                LastKnownLocation = x;
                locationManager.UpdateAddressAsync();
            }));

            AddDisposable(this.locationManager.AddressChanges.Subscribe(x => LastKnownAddress = x));
        }

        public GeoPosition LastKnownLocation
        {
            get { return lastKnownLocation; }
            private set { SetAndNotifyIfChanged(ref lastKnownLocation, value); }
        }

        public Address LastKnownAddress
        {
            get
            {
                return lastKnownAddress == Address.UnknownAddress
                    ? new Address(GeoPosition.ZeroPosition, CommonStrings.DeterminingLocation)
                    : lastKnownAddress;
            }
            private set { SetAndNotifyIfChanged(ref lastKnownAddress, value); }
        }

        public Task EnsureRecentLocation()
        {
            return locationManager.EnsureRecentLocationAsync(Recency);
        }

        public Task EnsureRecentAddress()
        {
            return locationManager.EnsureRecentAddressAsync(Recency);
        }

        public void StartListeningForGeoUpdates()
        {
            if (listener == null)
                listener = locationManager.Listen();

            LastKnownLocation = locationManager.LastKnownPosition;
            LastKnownAddress = locationManager.LastKnownAddress;
        }

        public void StopListeningForGeoUpdates()
        {
            if (listener != null)
            {
                listener.Dispose();
                listener = null;
            }
        }
    }
}
