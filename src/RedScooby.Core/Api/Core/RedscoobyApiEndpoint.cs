// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.IO;
using System.Net.Http.Formatting;

namespace RedScooby.Api.Core
{
    public abstract class RedscoobyApiEndpoint : IDisposable
    {
        protected RedscoobyApiEndpoint(RedScoobyHttpClient httpClient, MediaTypeFormatter formatter,
            string endpointPath = "")
        {
            if (endpointPath == null)
                throw new ArgumentNullException("endpointPath");

            EndpointPath = endpointPath;
            Client = httpClient;
            Formatter = formatter;
        }

        protected RedscoobyApiEndpoint(RedscoobyApiEndpoint parentEndpoint, string endpointPath = "")
            : this(parentEndpoint.Client, parentEndpoint.Formatter,
                Path.Combine(parentEndpoint.EndpointPath, endpointPath)) { }

        protected RedscoobyApiEndpoint(RedScoobyApiConfiguration configuration, RedScoobyHttpClient client,
            string endpointPath = "")
            : this(client, configuration.MediaTypeFormatter, endpointPath) { }

        protected string EndpointPath { get; private set; }
        protected RedScoobyHttpClient Client { get; set; }
        protected MediaTypeFormatter Formatter { get; set; }
        public virtual void Dispose() { }

        public virtual string GetApiPath(string resourcePath)
        {
            if (resourcePath == null)
                throw new ArgumentNullException("resourcePath");

            if (resourcePath.StartsWith("/") && EndpointPath.EndsWith("/"))
                return EndpointPath + resourcePath.Remove(0, 1);

            return EndpointPath + resourcePath;
        }
    }
}
