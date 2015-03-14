// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RedScooby.Data.Models;
using RedScooby.Logging;

namespace RedScooby.Data
{
    public class CountryInfoProvider
    {
        private readonly ICountryInfoStore store;

        public CountryInfoProvider(ICountryInfoStore store)
        {
            this.store = store;
        }

        public async Task<IDictionary<string, CountryInfo>> GetCountryInfo(bool forceRepopulate = false)
        {
            Log.Trace("Populating country list");

            IDictionary<string, CountryInfo> data;

            using (var stream = await store.GetCountryInfoStreamAsync())
            using (var reader = new StreamReader(stream))
                data = JsonConvert.DeserializeObject<IDictionary<string, CountryInfo>>(reader.ReadToEnd());

            return data;
        }
    }
}
