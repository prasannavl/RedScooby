// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Infrastructure.Networking;

namespace RedScooby.Api.Data
{
    public class ReceptionInfo
    {
        public MobileNetworkReception SignalLevel { get; set; }
        public string ServiceProvider { get; set; }
    }
}
