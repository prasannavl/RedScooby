// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Threading.Tasks;
using RedScooby.Api.Core;
using RedScooby.Infrastructure.Location;
using RedScooby.Logging;
using RedScooby.Utilities;

namespace RedScooby.Api.Endpoints
{
    public class MeEndpoint : RedscoobyApiEndpoint
    {
        private static readonly ILogger Log = Logging.Log.ForContext<MeEndpoint>();

        internal MeEndpoint(RedscoobyApiEndpoint parent)
            : base(parent, "me") { }

        public Task UpdateLocation(GeoPosition position)
        {
            var url = GetApiPath("/location");
            Log.Trace(f => f(url + " => {0}", position));

            return TaskCache.Completed;
        }
    }
}
