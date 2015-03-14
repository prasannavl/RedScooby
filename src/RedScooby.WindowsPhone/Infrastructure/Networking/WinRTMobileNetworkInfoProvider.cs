// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace RedScooby.Infrastructure.Networking
{
    internal class WinRtMobileNetworkInfoProvider : IMobileNetworkInfoProvider
    {
        public void Dispose() { }

        public Task<bool> IsMobileNetworkAvailableAsync()
        {
            return NetworkInformation.FindConnectionProfilesAsync(
                new ConnectionProfileFilter
                {
                    IsWwanConnectionProfile = true,
                    IsConnected = true
                })
                .AsTask()
                .ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted)
                            return false;

                        var profiles = t.Result;
                        if (profiles == null || profiles.Count == 0)
                            return false;

                        return true;
                    });
        }

        public Task<string> GetNetworkProviderIdAsync()
        {
            return NetworkInformation.FindConnectionProfilesAsync(
                new ConnectionProfileFilter
                {
                    IsWwanConnectionProfile = true,
                    IsConnected = true
                })
                .AsTask()
                .ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted)
                            return null;

                        var profiles = t.Result;
                        if (profiles == null || profiles.Count == 0)
                            return null;

                        foreach (var profile in profiles)
                        {
                            var signal = profile.GetSignalBars();
                            if (signal.HasValue)
                                return profile.WwanConnectionProfileDetails.HomeProviderId;
                        }

                        return null;
                    });
        }

        public Task<MobileNetworkReception> GetReceptionAsync()
        {
            return NetworkInformation.FindConnectionProfilesAsync(
                new ConnectionProfileFilter
                {
                    IsWwanConnectionProfile = true,
                    IsConnected = true
                })
                .AsTask()
                .ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted)
                            return MobileNetworkReception.Unknown;

                        var profiles = t.Result;
                        if (profiles == null || profiles.Count == 0)
                            return MobileNetworkReception.Unavailable;

                        foreach (var profile in profiles)
                        {
                            var signal = profile.GetSignalBars();
                            if (signal.HasValue)
                                return (MobileNetworkReception) signal.Value;
                        }

                        return MobileNetworkReception.Unavailable;
                    });
        }

        public IObservable<MobileNetworkReception> MobileNetworkReceptionChanges
        {
            get { throw new NotImplementedException(); }
        }
    }
}
