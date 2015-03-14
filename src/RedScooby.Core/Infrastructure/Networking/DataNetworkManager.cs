// Author: Prasanna V. Loganathar
// Created: 6:51 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using RedScooby.Utilities;

namespace RedScooby.Infrastructure.Networking
{
    public class DataNetworkManager
    {
        private readonly IDataNetworkInfoProvider infoProvider;

        public DataNetworkManager(IDataNetworkInfoProvider infoProvider)
        {
            this.infoProvider = infoProvider;
            Client = new QueuedHttpClient();
        }

        public QueuedHttpClient Client { get; private set; }

        public IObservable<DataNetworkStatus> DataNetworkStatusChanges
        {
            get { return infoProvider.DataNetworkStatusChanges; }
        }

        public Task<DataNetworkSpeed> GetDataNetworkSpeedAsync(bool testActualSpeed = false)
        {
            return infoProvider.GetDataNetworkSpeedAsync(testActualSpeed);
        }

        public Task<DataNetworkType> GetDataNetworkTypeAsync()
        {
            return infoProvider.GetDataNetworkTypeAsync();
        }

        public Task<bool> IsDataNetworkAvailableAsync(bool performIcmpTest = true)
        {
            return infoProvider.IsDataNetworkAvailableAsync(performIcmpTest);
        }
    }
}
