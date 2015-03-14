// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Api.Core;

namespace RedScooby.Api
{
    public static class RedScoobyApi
    {
        public static RedScoobyApiClient Client { get; private set; }

        public static RedScoobyApiClient CreateClient(RedScoobyApiConfiguration config)
        {
            return new RedScoobyApiClient(config, new RedScoobyHttpClient(), "http://www.redscooby.com/api/");
        }

        public static void Initialize(RedScoobyApiConfiguration config)
        {
            Client = CreateClient(config);
        }
    }
}
