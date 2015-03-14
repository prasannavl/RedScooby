// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using RedScooby.Utilities;

namespace RedScooby.Infrastructure.Networking
{
    internal class WinRtDataNetworkInfoProvider : IDataNetworkInfoProvider
    {
        private readonly IDisposable subscription;
        private readonly Subject<DataNetworkStatus> dataNetworkStatusSubject;

        public WinRtDataNetworkInfoProvider()
        {
            dataNetworkStatusSubject = new Subject<DataNetworkStatus>();
            NetworkStatusChangedEventHandler handler = null;

            handler = sender => { dataNetworkStatusSubject.OnNext(GetStatus()); };

            NetworkInformation.NetworkStatusChanged += handler;
            subscription = Disposable.Create(() => { NetworkInformation.NetworkStatusChanged -= handler; });
        }

        public Task<DataNetworkType> GetDataNetworkTypeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DataNetworkSpeed> GetDataNetworkSpeedAsync(bool testActualSpeed = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsDataNetworkAvailableAsync(bool checkForInternetAccess = true)
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile == null)
                return TaskCache.False;

            if (checkForInternetAccess)
            {
                var level = profile.GetNetworkConnectivityLevel();
                if (level == NetworkConnectivityLevel.ConstrainedInternetAccess ||
                    level == NetworkConnectivityLevel.InternetAccess)
                {
                    return TaskCache.True;
                }
                return TaskCache.False;
            }
            return TaskCache.True;
        }

        public IObservable<DataNetworkStatus> DataNetworkStatusChanges
        {
            get { return dataNetworkStatusSubject.AsObservable(); }
        }

        public void Dispose()
        {
            subscription.Dispose();
        }

        private DataNetworkStatus GetStatus()
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();

            if (profile == null)
                return DataNetworkStatus.Disconnected;

            var level = profile.GetNetworkConnectivityLevel();
            switch (level)
            {
                case NetworkConnectivityLevel.LocalAccess:
                    return DataNetworkStatus.LocalAccess;
                case NetworkConnectivityLevel.InternetAccess:
                    return DataNetworkStatus.InternetAccess;
                case NetworkConnectivityLevel.ConstrainedInternetAccess:
                    return DataNetworkStatus.ConstrainedInternetAccess;
                default:
                    return DataNetworkStatus.Disconnected;
            }
        }
    }
}
