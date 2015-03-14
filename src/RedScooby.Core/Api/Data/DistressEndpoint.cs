// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using RedScooby.Components;
using RedScooby.Infrastructure.Location;

namespace RedScooby.Api.Data
{
    public class DistressInfo
    {
        public bool IsCovertMode { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public GeoPosition Position { get; set; }
        public DistressNotificationPreference NotificationPreference { get; set; }
        public ReceptionInfo ReceptionInfo { get; set; }
        public double BatteryRemaining { get; set; }
        public DistressCategory Category { get; set; }
    }
}
