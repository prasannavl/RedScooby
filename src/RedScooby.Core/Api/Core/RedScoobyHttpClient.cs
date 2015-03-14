// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Utilities;

namespace RedScooby.Api.Core
{
    public class RedScoobyHttpClient : QueuedHttpClient
    {
        private void SetApiCredentials(string apiKey, string authToken)
        {
            // Generate token from creds
            var apiToken = "";
            DefaultRequestHeaders.TryAddWithoutValidation("X-RS-Token", apiToken);
        }
    }
}
