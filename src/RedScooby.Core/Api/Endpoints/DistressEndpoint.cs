// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using RedScooby.Api.Core;
using RedScooby.Api.Data;
using RedScooby.Logging;
using RedScooby.Utilities;

namespace RedScooby.Api.Endpoints
{
    public class DistressEndpoint : RedscoobyApiEndpoint
    {
        private static readonly ILogger Log = Logging.Log.ForContext<DistressEndpoint>();

        internal DistressEndpoint(RedscoobyApiEndpoint parent)
            : base(parent, "distress") { }

        public Task Activate(DateTimeOffset timestamp)
        {
            var url = GetApiPath("/activate");
            Log.Trace(f => f(url + " => {0}", timestamp.ToString()));

            return TaskCache.Completed;
        }

        public Task ActivateCountdown(DateTimeOffset timestamp)
        {
            var url = GetApiPath("/startcount");
            Log.Trace(f => f(url + " => {0}", timestamp.ToString()));

            return TaskCache.Completed;
        }

        public Task Deactivate(DateTimeOffset timestamp)
        {
            var url = GetApiPath("/deactivate");
            Log.Trace(f => f(url + " => {0}", timestamp.ToString()));

            return TaskCache.Completed;
        }

        public Task DeactivateCountdown(DateTimeOffset timestamp)
        {
            var url = GetApiPath("/cancel");
            Log.Trace(f => f(url + " => {0}", timestamp.ToString()));

            return TaskCache.Completed;
        }

        public Task Report(DistressInfo info)
        {
            var url = GetApiPath("/report");
            Log.Trace(f => f(url + " => {0}", info));
            return TaskCache.Completed;
        }
    }
}
