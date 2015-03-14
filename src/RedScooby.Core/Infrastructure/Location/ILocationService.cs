// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;

namespace RedScooby.Infrastructure.Location
{
    public interface ILocationService : IDisposable
    {
        double DesiredAccuracyMeters { get; set; }
        GeoPosition LastKnownPosition { get; }
        double MovementThresholdMeters { get; set; }
        IObservable<GeoPosition> PositionChanges { get; }
        TimeSpan ReportInterval { get; set; }
        LocationServiceStatus Status { get; }
        IObservable<LocationServiceStatus> StatusChanges { get; }
        Task<GeoPosition> GetPositionAsync();
        void StartListening();
        void StopListening();
    }

    public class GeoPosition
    {
        public static GeoPosition ZeroPosition = new GeoPosition
        {
            HorizontalAccuracy = 0,
            Latitude = 0,
            Longitude = 0,
            Timestamp = new DateTimeOffset()
        };

        public DateTimeOffset Timestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public double? VerticalAccuracy { get; set; }
        public double HorizontalAccuracy { get; set; }
        public double? Course { get; set; }
        public double? Speed { get; set; }
    }

    public enum LocationServiceStatus
    {
        Active,
        Initializing,
        NoData,
        Disabled,
        NotInitialized,
        NotAvailable
    }
}
