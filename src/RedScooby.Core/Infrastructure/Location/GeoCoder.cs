// Author: Prasanna V. Loganathar
// Created: 6:49 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using RedScooby.Data.Models;
using RedScooby.Infrastructure.Networking;
using RedScooby.Logging;
using RedScooby.Utilities;

namespace RedScooby.Infrastructure.Location
{
    public class GeoCoder
    {
        //TODO: Low: Implement a persistant MRU cache of the last 100 approximate addresses 
        // to avoid data costs.

        private const string apiUrl =
            "http://maps.googleapis.com/maps/api/geocode/json?language=en&latlng={0},{1}";

        private const string geoCoderNetworkConcurrencyGroupKey = "geocoder";
        private static readonly ILogger Log = Logging.Log.ForContext<GeoCoder>();
        private readonly DataNetworkManager dataNetworkManager;

        public GeoCoder(DataNetworkManager dataNetworkManager)
        {
            this.dataNetworkManager = dataNetworkManager;
        }

        public async Task<string> GetAddress(GeoPosition position)
        {
            Log.Trace("Request address for {0}, {1}", position.Latitude.ToString(), position.Longitude.ToString());
            var url = string.Format(apiUrl, position.Latitude.ToString(), position.Longitude.ToString());

            try
            {
                var resp =
                    await
                        dataNetworkManager.Client.GetAsync(url, NetworkDataPriority.Low,
                            geoCoderNetworkConcurrencyGroupKey);

                var result = await resp.Content.ReadAsAsync<GoogleAddressResult>();

                Log.Trace(f => f("Address response: {0}", result));

                if (result.Status == GoogleAddressResult.OkStatus)
                {
                    if (result.Results.Count > 0)
                    {
                        return result.Results.First().FormattedAddress;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(f => f(ex));
            }

            return null;
        }
    }
}
