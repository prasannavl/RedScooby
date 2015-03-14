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
    public class ConcernEndpoint : RedscoobyApiEndpoint
    {
        private static readonly ILogger Log = Logging.Log.ForContext<ConcernEndpoint>();

        internal ConcernEndpoint(RedscoobyApiEndpoint parent)
            : base(parent, "concern") { }

        public Task Activate(DateTimeOffset timestamp)
        {
            var url = GetApiPath("/activate");
            Log.Trace(f => f(url + " => {0}", timestamp));

            return TaskCache.Completed;
        }

        public Task Deactivate(DateTimeOffset timestamp)
        {
            var url = GetApiPath("/deactivate");
            Log.Trace(f => f(url + " => {0}", timestamp));

            return TaskCache.Completed;
        }

        public Task Report(ConcernInfo info)
        {
            var url = GetApiPath("/report");
            Log.Trace(f => f(url + " => {0}", info));
            return TaskCache.Completed;
        }
    }
}
