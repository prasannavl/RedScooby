// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Net.Http.Formatting;

namespace RedScooby.Api.Core
{
    public class RedScoobyApiConfiguration
    {
        public RedScoobyApiConfiguration(string apiKey, string authToken,
            MediaTypeFormatter mediaTypeFormatter = null)
        {
            if (mediaTypeFormatter == null) mediaTypeFormatter = new JsonMediaTypeFormatter();

            MediaTypeFormatter = mediaTypeFormatter;
        }

        public MediaTypeFormatter MediaTypeFormatter { get; private set; }
    }
}
