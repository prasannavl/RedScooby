// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace RedScooby.Infrastructure.Location
{
    public class WinRtLocationService : ILocationService
    {
        private readonly Geolocator locator;
        private readonly Subject<GeoPosition> positionChanges;
        private readonly TypedEventHandler<Geolocator, PositionChangedEventArgs> positionChangedSubscription;
        private readonly Subject<LocationServiceStatus> statusChanges;
        private readonly TypedEventHandler<Geolocator, StatusChangedEventArgs> statusChangedSubscription;
        private TimeSpan? lastReportInterval;

        public WinRtLocationService()
        {
            locator = new Geolocator();

            positionChanges = new Subject<GeoPosition>();
            statusChanges = new Subject<LocationServiceStatus>();

            statusChangedSubscription =
                (sender, args) => statusChanges.OnNext(args.Status.ToLocationServiceStatus());

            positionChangedSubscription =
                (sender, args) =>
                {
                    var pos = args.Position.Coordinate.ToGeoPosition();
                    positionChanges.OnNext(pos);
                    LastKnownPosition = pos;
                };
        }

        public void Dispose() { }

        public void StartListening()
        {
            if (lastReportInterval.HasValue)
            {
                ReportInterval = lastReportInterval.Value;
            }

            locator.StatusChanged += statusChangedSubscription;
            locator.PositionChanged += positionChangedSubscription;
        }

        public void StopListening()
        {
            locator.StatusChanged -= statusChangedSubscription;
            locator.PositionChanged -= positionChangedSubscription;

            lastReportInterval = ReportInterval;
            ReportInterval = TimeSpan.FromMinutes(30);
        }

        public async Task<GeoPosition> GetPositionAsync()
        {
            var pos = (await locator.GetGeopositionAsync()).ToGeoPosition();
            LastKnownPosition = pos;

            return pos;
        }

        public double DesiredAccuracyMeters
        {
            get { return locator.DesiredAccuracyInMeters.GetValueOrDefault(); }
            set { locator.DesiredAccuracyInMeters = (uint) value; }
        }

        public LocationServiceStatus Status
        {
            get { return locator.LocationStatus.ToLocationServiceStatus(); }
        }

        public GeoPosition LastKnownPosition { get; private set; }

        public double MovementThresholdMeters
        {
            get { return locator.MovementThreshold; }
            set { locator.MovementThreshold = value; }
        }

        public TimeSpan ReportInterval
        {
            get { return TimeSpan.FromMilliseconds(locator.ReportInterval); }
            set { locator.ReportInterval = (uint) value.TotalMilliseconds; }
        }

        public IObservable<GeoPosition> PositionChanges
        {
            get { return positionChanges; }
        }

        public IObservable<LocationServiceStatus> StatusChanges
        {
            get { return statusChanges; }
        }
    }

    public static class LocationServiceExtensions
    {
        public static GeoPosition ToGeoPosition(this Geoposition position)
        {
            if (position == null) throw new ArgumentNullException("position");

            return new GeoPosition
            {
                Altitude = position.Coordinate.Point.Position.Altitude,
                Course = position.Coordinate.Heading,
                HorizontalAccuracy = position.Coordinate.Accuracy,
                Latitude = position.Coordinate.Point.Position.Latitude,
                Longitude = position.Coordinate.Point.Position.Longitude,
                Speed = position.Coordinate.Speed,
                Timestamp = position.Coordinate.Timestamp,
                VerticalAccuracy = position.Coordinate.AltitudeAccuracy
            };
        }

        public static GeoPosition ToGeoPosition(this Geocoordinate position)
        {
            if (position == null) throw new ArgumentNullException("position");

            return new GeoPosition
            {
                Altitude = position.Point.Position.Altitude,
                Course = position.Heading,
                HorizontalAccuracy = position.Accuracy,
                Latitude = position.Point.Position.Latitude,
                Longitude = position.Point.Position.Longitude,
                Speed = position.Speed,
                Timestamp = position.Timestamp,
                VerticalAccuracy = position.AltitudeAccuracy
            };
        }

        public static LocationServiceStatus ToLocationServiceStatus(this PositionStatus status)
        {
            return (LocationServiceStatus) status;
        }
    }
}
