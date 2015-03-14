// Author: Prasanna V. Loganathar
// Created: 6:50 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using RedScooby.Helpers;
using RedScooby.Logging;
using RedScooby.Utilities;

namespace RedScooby.Infrastructure.Location
{
    public class LocationManager : IDisposable
    {
        public static TimeSpan DefaultRecentLocationTimeSpan = TimeSpan.FromSeconds(60);
        public static TimeSpan DefaultRecentAddressTimeSpan = DefaultRecentLocationTimeSpan;
        // ReSharper disable once InconsistentNaming
        private static readonly ILogger Log = Logging.Log.ForContext<LocationManager>();
        private readonly SemaphoreSlim addressUpdateSemaphore;
        private readonly SemaphoreSlim locationUpdateSemaphore;
        private readonly GeoCoder geoCoder;
        private readonly ILocationService service;
        private readonly IDisposable positionChangesSubscription;
        private readonly Subject<GeoPosition> positionChangesSubject;
        private readonly Subject<Address> addressChangesSubject;
        private readonly object listenerSyncRoot = new object();
        private IDisposable locationChangeSubscription;
        private int listeners;
        private int locationUpdatesInProgress;
        private int locationSyncPoint;
        private volatile int locationUpdateSemaphoreCount;
        private int addressSyncPoint;
        private int addressUpdatesInProgress;
        private volatile int addressUpdateSemaphoreCount;
        private Address lastKnownAddress;
        private GeoPosition lastKnownPosition;

        public LocationManager(ILocationService service, GeoCoder geoCoder)
        {
            Log.Info("Init");

            this.service = service;
            this.geoCoder = geoCoder;

            // Initialize with zero, since its never allowed without blocking. 
            // The sempahore is brought into the picture only if an actions occurs
            // concurrently, otherwise the entire semaphore logic is bypassed.

            locationUpdateSemaphore = new SemaphoreSlim(0);
            addressUpdateSemaphore = new SemaphoreSlim(0);
            service.MovementThresholdMeters = 8;


            positionChangesSubject = new Subject<GeoPosition>();
            addressChangesSubject = new Subject<Address>();

            LastKnownPosition = GeoPosition.ZeroPosition;
            LastKnownAddress = Address.UnknownAddress;
            positionChangesSubscription = service.PositionChanges.Subscribe(positionChangesSubject);
        }

        public bool IsListening { get; private set; }

        public IObservable<LocationServiceStatus> StatusChanges
        {
            get { return service.StatusChanges; }
        }

        public IObservable<GeoPosition> PositionChanges
        {
            get { return positionChangesSubject.AsObservable(); }
        }

        public IObservable<Address> AddressChanges
        {
            get { return addressChangesSubject.AsObservable(); }
        }

        public Address LastKnownAddress
        {
            get { return lastKnownAddress; }
            private set
            {
                lastKnownAddress = value;
                addressChangesSubject.OnNext(value);
            }
        }

        public GeoPosition LastKnownPosition
        {
            get { return lastKnownPosition; }
            private set
            {
                lastKnownPosition = value;
                positionChangesSubject.OnNext(value);
            }
        }

        public void Dispose()
        {
            Log.Info("Dispose");

            DisposeLocationChangeSubscription();
            positionChangesSubscription.Dispose();
            addressChangesSubject.Dispose();
            locationUpdateSemaphore.Dispose();
            addressUpdateSemaphore.Dispose();
            service.Dispose();
        }

        /// <summary>
        ///     Ensures a recent address within the last 60 seconds.
        /// </summary>
        /// <returns></returns>
        public Task EnsureRecentAddressAsync()
        {
            return EnsureRecentAddressAsync(DefaultRecentAddressTimeSpan);
        }

        public async Task EnsureRecentAddressAsync(TimeSpan recentTimeSpan)
        {
            Log.Trace("Ensure recent address within: {0}", recentTimeSpan.ToString());

            if (LastKnownAddress == Address.UnknownAddress ||
                DateTimeOffset.Now.Subtract(recentTimeSpan) > LastKnownAddress.Position.Timestamp)
            {
                Log.Trace("Waiting for recent address");

                await EnsureRecentLocationAsync(recentTimeSpan);
                await UpdateAddressAsync();
            }

            Log.Trace("Address recent guaranteed");
        }

        /// <summary>
        ///     Ensures a recent location within the last 60 seconds.
        /// </summary>
        /// <returns></returns>
        public Task EnsureRecentLocationAsync()
        {
            return EnsureRecentLocationAsync(DefaultRecentLocationTimeSpan);
        }

        public async Task EnsureRecentLocationAsync(TimeSpan recentTimeSpan)
        {
            Log.Trace("Ensure recent location within: {0}", recentTimeSpan.ToString());

            if (DateTimeOffset.Now.Subtract(recentTimeSpan) > LastKnownPosition.Timestamp)
            {
                Log.Trace("Waiting for recent location");
                await UpdateLocationAsync();
            }

            Log.Trace("Location recent guaranteed");
        }

        public IDisposable Listen()
        {
            lock (listenerSyncRoot)
            {
                listeners++;
                if (!IsListening)
                {
                    StartListeningInternal();
                }
            }
            return DisposableHelpers.Create(self =>
            {
                lock (listenerSyncRoot)
                {
                    listeners--;
                    if (listeners == 0)
                        self.StopListeningInternal();
                }
            }, this);
        }

        /// <summary>
        ///     Globally starts the listener
        /// </summary>
        public void StartListening()
        {
            lock (listenerSyncRoot)
            {
                StartListeningInternal();
            }
        }

        /// <summary>
        ///     Globally stops the listener
        /// </summary>
        public void StopListening()
        {
            lock (listenerSyncRoot)
            {
                StopListeningInternal();
            }
        }

        public async Task UpdateLocationAsync()
        {
            if (Interlocked.CompareExchange(ref locationUpdatesInProgress, 1, 0) == 0)
            {
                Log.Trace("Updating location");

                LastKnownPosition = await service.GetPositionAsync();

                InterlockedHelpers.SpinWaitUntilCompareExchangeSucceeds(ref locationSyncPoint, 1, 0);
                // No need to sync semaphore count since its always used in a critical section
                if (locationUpdateSemaphoreCount > 0)
                {
                    Log.Trace("Releasing " + locationUpdateSemaphoreCount.ToString() + " location update waiters");
                    locationUpdateSemaphore.Release(locationUpdateSemaphoreCount);
                    locationUpdateSemaphoreCount = 0;
                }
                Interlocked.Exchange(ref locationUpdatesInProgress, 0);
                Interlocked.Exchange(ref locationSyncPoint, 0);
            }
            else
            {
                InterlockedHelpers.SpinWaitUntilCompareExchangeSucceeds(ref locationSyncPoint, 1, 0);
                // Update progress can be modified outside sync points.
                // Use volatile read to be safe.
                if (Volatile.Read(ref locationUpdatesInProgress) != 0)
                {
                    Log.Trace("Waiting for location");

                    locationUpdateSemaphoreCount++;
                    var res = locationUpdateSemaphore.WaitAsync();
                    Interlocked.Exchange(ref locationSyncPoint, 0);
                    await res;
                    return;
                }
                Interlocked.Exchange(ref locationSyncPoint, 0);
            }
        }

        public Task UpdateAddressAsync()
        {
            return LastKnownAddress.Position != LastKnownPosition
                ? UpdateAddressInternalAsync()
                : TaskCache.Completed;
        }

        private void StartListeningInternal()
        {
            Log.Info("Start listening");

            service.StartListening();
            IsListening = true;

            DisposeLocationChangeSubscription();
            locationChangeSubscription = service.PositionChanges.Subscribe(x => LastKnownPosition = x);

            Log.Info("Started listening");
        }

        private void StopListeningInternal()
        {
            Log.Info("Stop listening");

            service.StopListening();
            IsListening = false;

            DisposeLocationChangeSubscription();

            Log.Info("Stopped listening");
        }

        private void DisposeLocationChangeSubscription()
        {
            if (locationChangeSubscription != null)
            {
                Log.Trace("Dispose location change subscription");
                locationChangeSubscription.Dispose();
                locationChangeSubscription = null;
            }
        }

        private async Task UpdateAddressInternalAsync()
        {
            if (Interlocked.CompareExchange(ref addressUpdatesInProgress, 1, 0) == 0)
            {
                Log.Trace("Updating address");

                var lastPos = LastKnownPosition;
                var addr = await geoCoder.GetAddress(lastPos) ?? Address.UnknownAddressString;
                LastKnownAddress = new Address(lastPos, addr);

                InterlockedHelpers.SpinWaitUntilCompareExchangeSucceeds(ref addressSyncPoint, 1, 0);
                // No need to sync semaphore count since its always used in a critical section
                if (addressUpdateSemaphoreCount > 0)
                {
                    Log.Trace("Releasing " + addressUpdateSemaphoreCount.ToString() + " address update waiters");
                    addressUpdateSemaphore.Release(addressUpdateSemaphoreCount);
                    addressUpdateSemaphoreCount = 0;
                }
                Interlocked.Exchange(ref addressUpdatesInProgress, 0);
                Interlocked.Exchange(ref addressSyncPoint, 0);
            }
            else
            {
                InterlockedHelpers.SpinWaitUntilCompareExchangeSucceeds(ref addressSyncPoint, 1, 0);
                // Update progress can be modified outside sync points.
                // Use volatile read to be safe.
                if (Volatile.Read(ref addressUpdatesInProgress) != 0)
                {
                    Log.Trace("Waiting for address");

                    addressUpdateSemaphoreCount++;
                    var res = addressUpdateSemaphore.WaitAsync();
                    Interlocked.Exchange(ref addressSyncPoint, 0);
                    await res;
                    return;
                }

                Interlocked.Exchange(ref addressSyncPoint, 0);
            }
        }
    }

    public class Address
    {
        public const string UnknownAddressString = "Address unavailable.";
        public static Address UnknownAddress = new Address(GeoPosition.ZeroPosition, UnknownAddressString);

        public Address(GeoPosition position, string value)
        {
            Position = position;
            Value = value;
        }

        public GeoPosition Position { get; private set; }
        public string Value { get; private set; }

        public override string ToString()
        {
            return Value != null ? Value.ToString() : string.Empty;
        }
    }
}
