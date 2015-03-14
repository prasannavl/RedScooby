// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Runtime.Serialization;
using System.Threading.Tasks;
using RedScooby.Data.Core;
using RedScooby.Data.Tables;
using RedScooby.Infrastructure.Flashlight;

namespace RedScooby.Models
{
    public sealed class LocalSettingsModel : PersistentDispatchingObject
    {
        private bool autoVoiceOnDistress;
        private bool isDeveloperModeActive;
        private bool isDemoModeActive;
        private ConcernStatePersistence lastConcernState;
        private DistressPersistenceState lastDistressState;
        private FeedbackPersistenceState lastFeedbackState;
        private bool isFirstRun;

        public LocalSettingsModel()
        {
            isFirstRun = true;
        }

        public LocalSettingsModel(StreamingContext context) : base(null) { }

        public bool IsFirstRun
        {
            get { return isFirstRun; }
            set { SetAndNotifyIfChanged(ref isFirstRun, value); }
        }

        public bool IsDemoModeActive
        {
            get { return isDemoModeActive; }
            set { SetAndNotifyIfChanged(ref isDemoModeActive, value); }
        }

        public bool IsDeveloperModeActive
        {
            get { return isDeveloperModeActive; }
            set { SetAndNotifyIfChanged(ref isDeveloperModeActive, value); }
        }

        public ConcernStatePersistence LastConcernState
        {
            get { return lastConcernState; }
            set { SetAndNotifyIfChanged(ref lastConcernState, value); }
        }

        public DistressPersistenceState LastDistressState
        {
            get { return lastDistressState; }
            set { SetAndNotifyIfChanged(ref lastDistressState, value); }
        }

        public FeedbackPersistenceState LastFeedbackState
        {
            get { return lastFeedbackState; }
            set { SetAndNotifyIfChanged(ref lastFeedbackState, value); }
        }

        public bool AutoVoiceOnDistress
        {
            get { return autoVoiceOnDistress; }
            set { SetAndNotifyIfChanged(ref autoVoiceOnDistress, value); }
        }

        #region WinRT specific settings

#if NETFX_CORE

        private bool useQuickFlashlightDriver;

        public bool UseQuickFlashlightDriver
        {
            get { return useQuickFlashlightDriver; }
            set
            {
                SetAndNotifyIfChanged(ref useQuickFlashlightDriver, value,
                    async () => { await SwapFlashlightService(value); });
            }
        }

        private async Task SwapFlashlightService(bool quickFlashLightDriver)
        {
            var proxy = App.Current.Services.Locate<IFlashlightService>() as FlashlightProxyService;
            if (proxy == null) return;

            var onState = false;
            await proxy.SwapService(
                quickFlashLightDriver
                    ? (IFlashlightService) new WinRtFlashlightService()
                    : new WinRtFlashlightFallbackService(),
                async (innerService) =>
                {
                    onState = innerService.IsOn;
                    if (onState)
                    {
                        await innerService.TurnOff();
                    }
                }, async (innerService) =>
                {
                    if (onState)
                    {
                        await innerService.TurnOn();
                    }
                });
        }
#endif

        #endregion
    }
}
