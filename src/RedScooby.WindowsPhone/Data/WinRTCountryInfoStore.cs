// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace RedScooby.Data
{
    internal sealed class WinRtCountryInfoStore : ICountryInfoStore
    {
        private const string path = @"Assets\data\countrycodes.json";

        public async Task<Stream> GetCountryInfoStreamAsync()
        {
            var file = await Package.Current.InstalledLocation.GetFileAsync(path).AsTask().ConfigureAwait(false);
            return await file.OpenStreamForReadAsync().ConfigureAwait(false);
        }
    }
}
