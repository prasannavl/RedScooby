// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;

namespace RedScooby.Infrastructure.Networking
{
    public interface IDataNetworkInfoProvider : IDisposable
    {
        IObservable<DataNetworkStatus> DataNetworkStatusChanges { get; }
        Task<DataNetworkSpeed> GetDataNetworkSpeedAsync(bool testActualSpeed = false);
        Task<DataNetworkType> GetDataNetworkTypeAsync();
        Task<bool> IsDataNetworkAvailableAsync(bool checkForInternetAccess = true);
    }

    public enum DataNetworkStatus : byte
    {
        InternetAccess,
        ConstrainedInternetAccess,
        LocalAccess,
        Disconnected
    }

    public enum DataNetworkSpeed : byte
    {
        Unknown,
        K56KOrLess,
        K56To256,
        K256To512,
        K512To1024,
        M1To4,
        M4To8,
        Fastest
    }

    public enum DataNetworkType : byte
    {
        Unknown,
        Wired,
        WiFi,
        Mobile2G,
        Mobile2To3G,
        Mobile3G,
        Mobile3To4G,
        Mobile4G
    }
}
