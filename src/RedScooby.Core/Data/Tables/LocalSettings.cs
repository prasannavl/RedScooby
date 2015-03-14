// Author: Prasanna V. Loganathar
// Created: 7:11 PM 04-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using RedScooby.Components;
using RedScooby.Models;
using SQLite.Net.Attributes;

namespace RedScooby.Data.Tables
{
    public class LocalSettings : DbObjectWithId<LocalSettingsModel>
    {
        public bool IsFirstRun { get; set; }
        public bool IsDeveloperModeActive { get; set; }
        public bool IsDemoModeActive { get; set; }
        public bool AutoVoiceOnDistress { get; set; }
        public bool HasActiveAssistState { get; set; }
#if NETFX_CORE
        public bool QuickFlashLightDriver { get; set; }
#endif

        public override void PopulateModel(LocalSettingsModel model)
        {
            base.PopulateModel(model);
            model.IsFirstRun = IsFirstRun;
            model.IsDeveloperModeActive = IsDeveloperModeActive;
            model.IsDemoModeActive = IsDemoModeActive;
            model.AutoVoiceOnDistress = AutoVoiceOnDistress;

#if NETFX_CORE
            model.UseQuickFlashlightDriver = QuickFlashLightDriver;
#endif
        }

        public override void PopulateFromModel(LocalSettingsModel model)
        {
            base.PopulateFromModel(model);
            IsFirstRun = model.IsFirstRun;
            IsDeveloperModeActive = model.IsDeveloperModeActive;
            IsDemoModeActive = model.IsDemoModeActive;
            AutoVoiceOnDistress = model.AutoVoiceOnDistress;

#if NETFX_CORE
            QuickFlashLightDriver = model.UseQuickFlashlightDriver;
#endif
        }
    }

    [Table("LocalSettingsConcernState")]
    public class ConcernStatePersistence : DbObjectWithId
    {
        public ConcernState State { get; set; }
        public TimeSpan FeedbackTimeInterval { get; set; }
        public DateTimeOffset NextFeedbackTime { get; set; }
        public ConcernNotificationPreference? CurrentNotificationPreference { get; set; }
    }

    [Table("LocalSettingsFeedbackState")]
    public class FeedbackPersistenceState : DbObjectWithId
    {
        public FeedbackManager.FeedbackContext Context { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public TimeSpan ExpiryDuration { get; set; }
        public bool FeedbackInProgress { get; set; }
    }

    [Table("LocalSettingsDistressState")]
    public class DistressPersistenceState : DbObjectWithId
    {
        public DistressState State { get; set; }
        public bool IsCovertOn { get; set; }
        public DateTimeOffset? NextCountdownExpiryTime { get; set; }
        public DistressCategory? DesiredCategory { get; set; }
        public DistressNotificationPreference? CurrentNotificationPreference { get; set; }
    }
}
