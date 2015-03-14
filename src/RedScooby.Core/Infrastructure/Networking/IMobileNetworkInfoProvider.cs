// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;

namespace RedScooby.Infrastructure.Networking
{
    public interface IMobileNetworkInfoProvider : IDisposable
    {
        IObservable<MobileNetworkReception> MobileNetworkReceptionChanges { get; }
        Task<string> GetNetworkProviderIdAsync();
        Task<MobileNetworkReception> GetReceptionAsync();
        Task<bool> IsMobileNetworkAvailableAsync();
    }

    public enum MobileNetworkReception
    {
        Unknown = -1,
        Unavailable = 0,
        Lowest = 1,
        Low = 2,
        Average = 3,
        High = 4,
        Highest = 5
    }
}
