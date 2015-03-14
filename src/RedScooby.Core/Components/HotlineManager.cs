// Author: Prasanna V. Loganathar
// Created: 7:14 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RedScooby.Api;
using RedScooby.Api.Data;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Battery;
using RedScooby.Infrastructure.Location;
using RedScooby.Infrastructure.Networking;
using RedScooby.Logging;
using RedScooby.Utilities;

namespace RedScooby.Components
{
    public class HotlineManager : IDisposable
    {
        public enum TrueReportingMode
        {
            Normal,
            Concern,
            Distress,
            CovertDistress,
        }

        public static TimeSpan PeriodicReportBackgroundTimeInterval = TimeSpan.FromMinutes(15);
        public static TimeSpan PeriodicReportActiveTimeInterval = TimeSpan.FromMinutes(3);
        private static readonly ILogger Log = Logging.Log.ForContext<HotlineManager>();
        private readonly object syncRoot = new object();
        private readonly LocationManager locationManager;
        private readonly Lazy<ConcernManager> concernManager;
        private readonly Lazy<DistressManager> distressManager;
        private readonly IMobileNetworkInfoProvider mobileNetworkInfoProvider;
        private readonly IBatteryInfoService batteryInfoService;
        private TrueReportingMode currentTrueReportingMode;
        private IDisposable periodicReporterSubscription;
        private DateTimeOffset lastReportedTime;
        private TimeSpan periodicReportInterval;
        private bool modeChangeFlag;
        private IDisposable locationListener;

        public HotlineManager(LocationManager locationManager,
            Lazy<ConcernManager> concernManager,
            Lazy<DistressManager> distressManager,
            IMobileNetworkInfoProvider mobileNetworkInfoProvider,
            IBatteryInfoService batteryInfoService)
        {
            this.locationManager = locationManager;
            this.concernManager = concernManager;
            this.distressManager = distressManager;
            this.mobileNetworkInfoProvider = mobileNetworkInfoProvider;
            this.batteryInfoService = batteryInfoService;

            ResetPeriodicReportingUnsafe();
        }

        public bool IsConcernActive
        {
            get { return concernManager.Value.GetCurrentState() == ConcernState.On; }
        }

        public bool IsDistressActive
        {
            get
            {
                return distressManager.Value.IsCovertDistressOn ||
                       distressManager.Value.GetCurrentState() != DistressState.Off;
            }
        }

        public void Dispose()
        {
            DisposePeriodReportingSubscription();
        }

        public void AutoAdapt()
        {
            Log.Trace("Auto adapting");

            lock (syncRoot)
            {
                var mode = DetectReportingMode();
                if (currentTrueReportingMode != mode)
                {
                    ChangeModeUnsafe(mode, false);
                }
                var ts = DetectInterval();
                if (ts != periodicReportInterval)
                    SetupReportTimer(ts);

                ReportIfRequiredAsync(periodicReportInterval)
                    .ContinueWithErrorHandling();
            }
        }

        public void ChangeMode(TrueReportingMode mode, bool reportNow = false)
        {
            if (currentTrueReportingMode != mode)
            {
                lock (syncRoot)
                {
                    ChangeModeUnsafe(mode, reportNow);
                }
            }
            else if (reportNow)
            {
                var ignore = ReportNowAsync(periodicReportInterval)
                    .ContinueWithErrorHandling();
            }
        }

        public void ChangeReportInterval(TimeSpan timeSpan)
        {
            if (periodicReportInterval != timeSpan)
            {
                lock (syncRoot)
                {
                    SetupReportTimer(timeSpan);
                }
            }
        }

        public TrueReportingMode DetectReportingMode()
        {
            if (distressManager.Value.IsCovertDistressOn)
                return TrueReportingMode.CovertDistress;

            if (IsDistressActive)
                return TrueReportingMode.Distress;

            return IsConcernActive ? TrueReportingMode.Concern : TrueReportingMode.Normal;
        }

        public Task ReportIfRequiredAsync(TimeSpan recency)
        {
            Log.Info("Report if required triggered");

            if (modeChangeFlag || lastReportedTime <= DateTimeOffset.Now.Subtract(recency) ||
                lastReportedTime >= DateTimeOffset.Now.Add(recency))
            {
                return ReportNowAsync(recency);
            }

            Log.Trace("Report not necessary");
            return TaskCache.Completed;
        }

        public Task ReportNowAsync()
        {
            var recency = periodicReportInterval;
            return ReportNowAsync(recency);
        }

        public async Task ReportNowAsync(TimeSpan locationRecency)
        {
            Log.Info("Reporting with recency: {0}", locationRecency);

            lastReportedTime = DateTimeOffset.Now;
            modeChangeFlag = false;

            TrueReportingMode mode;
            var t1 = locationManager.EnsureRecentLocationAsync(locationRecency).ConfigureAwait(false);

            lock (syncRoot)
            {
                mode = currentTrueReportingMode;
            }

            await t1;

            if (mode == TrueReportingMode.Normal)
            {
                await RedScoobyApi.Client.Me.UpdateLocation(locationManager.LastKnownPosition)
                    .ContinueWithErrorHandling().ConfigureAwait(false);
            }
            else
            {
                var signal = await mobileNetworkInfoProvider.GetReceptionAsync().ConfigureAwait(false);
                var provider = await mobileNetworkInfoProvider.GetNetworkProviderIdAsync().ConfigureAwait(false);

                var receptionInfo = new ReceptionInfo
                {
                    SignalLevel = signal,
                    ServiceProvider = provider ?? "",
                };

                if (mode == TrueReportingMode.Concern)
                {
                    var now = DateTimeOffset.Now;
                    var next = concernManager.Value.NextFeedbackTime;
                    var secondsLeft = 0.0;
                    if (next > now)
                    {
                        secondsLeft = (next - now).TotalSeconds;
                    }
                    await RedScoobyApi.Client.Concern.Report(new ConcernInfo
                    {
                        Timestamp = DateTimeOffset.Now,
                        Position = locationManager.LastKnownPosition,
                        NotificationPreference = concernManager.Value.CurrentNotificationPreference,
                        BatteryRemaining = batteryInfoService.GetRemainingChargePercentage(),
                        ReceptionInfo = receptionInfo,
                        SecondsLeft = secondsLeft,
                    }).ContinueWithErrorHandling().ConfigureAwait(false);
                }
                else
                {
                    await RedScoobyApi.Client.Distress.Report(
                        new DistressInfo
                        {
                            Timestamp = DateTimeOffset.Now,
                            Position = locationManager.LastKnownPosition,
                            NotificationPreference = distressManager.Value.CurrentNotificationPreference,
                            IsCovertMode = distressManager.Value.IsCovertDistressOn,
                            BatteryRemaining = batteryInfoService.GetRemainingChargePercentage(),
                            ReceptionInfo = receptionInfo,
                            Category = distressManager.Value.IsCovertDistressOn
                                ? DistressCategory.Defence
                                : distressManager.Value.DesiredCategory,
                        }).ContinueWithErrorHandling().ConfigureAwait(false);
                }
            }
        }

        public void ResetPeriodicReporting()
        {
            lock (syncRoot)
                ResetPeriodicReportingUnsafe();
        }

        public void SwitchReporting(TrueReportingMode mode, TimeSpan ts, bool delayReport = false)
        {
            lock (syncRoot)
            {
                if (mode != currentTrueReportingMode)
                    ChangeModeUnsafe(mode, false);

                if (ts != periodicReportInterval)
                    SetupReportTimer(ts);

                if (!delayReport)
                {
                    var ignore = ReportIfRequiredAsync(periodicReportInterval)
                        .ContinueWithErrorHandling();
                }
            }
        }

        private void DisposeLocationListener()
        {
            if (locationListener != null)
            {
                Log.Trace("Disposing location listener");
                locationListener.Dispose();
                locationListener = null;
            }
        }

        private void ChangeModeUnsafe(TrueReportingMode mode, bool reportNow = false)
        {
            Log.Trace("Mode change to : " + mode.ToString());

            if (mode == TrueReportingMode.Normal)
            {
                DisposeLocationListener();
            }
            else
            {
                if (locationListener == null)
                    locationListener = locationManager.Listen();
            }

            currentTrueReportingMode = mode;
            modeChangeFlag = true;

            if (reportNow)
            {
                var ignore = ReportNowAsync(periodicReportInterval);
            }
        }

        private TimeSpan DetectInterval()
        {
            var mode = currentTrueReportingMode;
            if (mode == TrueReportingMode.CovertDistress)
                return DistressManager.PeriodicReportCovertDistressTimeInterval;
            if (mode == TrueReportingMode.Distress)
                return DistressManager.PeriodicReportDistressTimeInterval;
            if (mode == TrueReportingMode.Concern)
                return ConcernManager.PeriodicReportTimeInterval;

            return App.Current.IsActive
                ? PeriodicReportActiveTimeInterval
                : PeriodicReportBackgroundTimeInterval;
        }

        private void DisposePeriodReportingSubscription()
        {
            if (periodicReporterSubscription != null)
            {
                Log.Info("Dispose periodic report subscription");

                periodicReporterSubscription.Dispose();
                periodicReporterSubscription = null;
            }
        }

        private void ResetPeriodicReportingUnsafe()
        {
            Log.Trace("Reseting periodic reporting");

            currentTrueReportingMode = TrueReportingMode.Normal;
            DisposeLocationListener();

            var targetInterval = App.Current.IsActive
                ? PeriodicReportActiveTimeInterval
                : PeriodicReportBackgroundTimeInterval;

            if (periodicReportInterval != targetInterval)
                SetupReportTimer(targetInterval);
        }

        private void SetupReportTimer(TimeSpan timeInterval)
        {
            Log.Trace("Change periodic report interval : {0}", timeInterval);

            DisposePeriodReportingSubscription();
            periodicReportInterval = timeInterval;

            periodicReporterSubscription = Observable
                .Timer(TimeSpan.Zero, periodicReportInterval)
                .Subscribe(x => { ReportIfRequiredAsync(periodicReportInterval); });
        }
    }
}
