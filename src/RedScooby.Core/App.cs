// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using RedScooby.Components;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Flashlight;
using RedScooby.Infrastructure.Siren;
using RedScooby.Models;
using RedScooby.Utilities;

namespace RedScooby
{
    public class App
    {
        public Action<bool, bool> ApplicationExitDelegate;
        public static App Current { get; set; }
        public bool IsActive { get; set; }
        public DependencyInjectionContainer Services { get; set; }
        public event Func<Task> Suspending;
        public event Func<Task> Resuming;

        public async Task ResumeAsync()
        {
            IsActive = true;
            await StartResumeSharedAsync();
            var handler = Resuming;
            if (handler != null)
                await handler();
        }

        public async Task ShutdownAsync()
        {
            await SuspendAsync();
            IsActive = false;
        }

        public async Task StartAsync()
        {
            IsActive = true;
            await StartResumeSharedAsync();
        }

        public async Task SuspendAsync()
        {
            SaveCoreStates();

            var handler = Suspending;
            if (handler != null)
                await handler();

            var saveTask = SaveAppModelAsync();

#if NETFX_CORE
            // WinRT doesn't allow code to run when screen is locked. So, stop and release flashlight to make sure it doesn't end up in an invalid state since it will disposed off anyway. May be required with iOS as well (or move this to the platform project).

            var flashlightControl = Current.Services.Locate<FlashlightControl>();
            await flashlightControl.ReleaseFlashlightAsync();
#endif
            await saveTask;

            IsActive = false;
        }

        public void SaveCoreStates()
        {
            var concernManager = Services.Locate<ConcernManager>();
            var distressManager = Services.Locate<DistressManager>();
            var feedbackManager = Services.Locate<FeedbackManager>();

            feedbackManager.SaveState();
            concernManager.SaveState();
            distressManager.SaveState();
        }

        public void RestoreCoreStates()
        {
            var concernManager = Services.Locate<ConcernManager>();
            var distressManager = Services.Locate<DistressManager>();
            var feedbackManager = Services.Locate<FeedbackManager>();

            feedbackManager.RestoreState();
            concernManager.RestoreState();
            distressManager.RestoreState();
        }

        public Task SaveAppModelAsync()
        {
            var appModel = Services.Locate<AppModel>();
            return appModel.SaveAllAsync();
        }

        private Task StartResumeSharedAsync()
        {
            // TODO: Only on user authenticated

            RestoreCoreStates();

            var hotlineManager = Services.Locate<HotlineManager>();
            var sirenControl = Services.Locate<SirenControl>();

            var _ = Task.Delay(750).ContinueWith(t => { var ix = sirenControl.SyncStateAsync(); })
                .ContinueWithErrorHandling();

            var ignore = hotlineManager.ReportIfRequiredAsync(TimeSpan.FromSeconds(30))
                .ContinueWithErrorHandling();

            return TaskCache.Completed;
        }
    }
}
