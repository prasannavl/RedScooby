// Author: Prasanna V. Loganathar
// Created: 12:47 AM 25-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Windows.Storage;
using Windows.UI.Popups;
using RedScooby.Common;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.Logging;
using RedScooby.Views.Settings;

namespace RedScooby.ViewModels
{
    public class WinRtAdvancedOptionsViewModel : ViewModelBase
    {
        public static bool IsPersistentLoggingEnabled;
        private static IDisposable _logSubscription;
        private readonly WinRtSettingsViewModel settingsViewModel;
        private IEnumerable<ErrorLogInfoWrapper> errorLogs;

        public WinRtAdvancedOptionsViewModel(WinRtSettingsViewModel settingsViewModel)
        {
            this.settingsViewModel = settingsViewModel;

            StartLoggingCommand = CommandFactory.Create(() =>
            {
                if (_logSubscription != null)
                    return;

                if (LogEvents == null)
                    LogEvents = new ChangeDispatchingLimitedQueue<string>(50);

                _logSubscription =
                    Log.Logger.Events.Subscribe(
                        x =>
                        {
                            var eventBuffer = LogEvents;
                            if (eventBuffer != null)
                                eventBuffer.Enqueue(string.Format("{0} [{1}]: {2}", x.Timestamp.ToString("t"),
                                    x.Level.ToString(), x.Message));
                        });

                Log.Info("Log observer added - LogView");
                if (!IsPersistentLoggingEnabled) Log.Info("Persistent logging not active");
            });

            var shouldWarnFullReset = true;

            FullAppResetAsyncCommand = CommandFactory.CreateAsync(async () =>
            {
                if (shouldWarnFullReset)
                {
                    var dialog =
                        new MessageDialog(
                            "If you do a full app reset when any of the assistance modes are active (either covert or regular), a distress call will be triggered, and will remain active until you log back in, and disable it." +
                            Environment.NewLine + Environment.NewLine +
                            "If you're sure you want to do a full app reset and that you've deactivated all the assistance modes properly, select the full app reset option again.",
                            "Warning!");
                    await dialog.ShowAsync();
                    shouldWarnFullReset = false;
                    AddDisposable(Observable.Timer(TimeSpan.FromSeconds(30)).Subscribe(x => shouldWarnFullReset = true));
                }
                else
                {
                    Log.Info("Performing app reset..");
                    try
                    {
                        await ApplicationData.Current.LocalFolder.DeleteAsync();
                        Log.Info("Full app reset done");
                    }
                    catch (Exception ex)
                    {
                        if (Debugger.IsAttached)
                            Debugger.Break();
                        Log.Error(f => f("Error occurred during full reset. Please uninstall, and reinstall the app." +
                                         "\r\nError details: ", ex));
                    }
                    App.Current.ApplicationExitDelegate.InvokeIfNotNullWith(false, true);
                }
            });

            var shouldWarnLogOut = true;

            ResetUserAsyncCommand = CommandFactory.CreateAsync(async () =>
            {
                if (shouldWarnLogOut)
                {
                    var dialog =
                        new MessageDialog(
                            "If you log out when any of the assistance modes are active (either covert or regular), a distress call will be triggered, and will remain active until you log back in, and disable it." +
                            Environment.NewLine + Environment.NewLine +
                            "If you're sure you want to log out and that you've deactivated all the assistance modes properly, select the log out option again.",
                            "Warning!");
                    await dialog.ShowAsync();
                    shouldWarnLogOut = false;
                    AddDisposable(Observable.Timer(TimeSpan.FromSeconds(30)).Subscribe(x => shouldWarnLogOut = true));
                }
                else
                {
                    this.settingsViewModel.CoreViewModel.ResetUserCommand.Execute(null);
                }
            });

            TogglePersistentLogging = CommandFactory.Create(() =>
            {
                Log.Info(IsPersistentLoggingEnabled ? "Persistent logging disabled" : "Persistent logging enabled");
                IsPersistentLoggingEnabled = !IsPersistentLoggingEnabled;
            });

            ViewErrorLogAsyncCommand = CommandFactory.CreateAsyncWithParameter<string>(async s =>
            {
                var data = await FileSystemLogger.Current.GetSavedLogDataAsync(s);
                await WindowHelpers.NavigateWithDataContextAsync(typeof (LogView), new {LogName = s, Data = data});
            });

            FileSystemLogger.Current.GetSavedLogListAsync().ContinueWith(t =>
            {
                t.HandleError();

                if (!t.IsFaulted)
                    ErrorLogs = t.Result
                        .Select(name => new ErrorLogInfoWrapper {Name = name, Command = ViewErrorLogAsyncCommand});
            });


            CrashApplicationCommand = CommandFactory.Create(
                () => { throw new Exception("Application crashed intentionally"); });
        }

        public static ChangeDispatchingLimitedQueue<string> LogEvents { get; private set; }
        public RelayCommand CrashApplicationCommand { get; private set; }
        public AsyncRelayCommand<string> ViewErrorLogAsyncCommand { get; private set; }

        public IEnumerable<ErrorLogInfoWrapper> ErrorLogs
        {
            get { return errorLogs; }
            private set { SetAndNotifyIfChanged(ref errorLogs, value); }
        }

        public RelayCommand TogglePersistentLogging { get; private set; }
        public AsyncRelayCommand ResetUserAsyncCommand { get; private set; }
        public AsyncRelayCommand ForceSaveDataAsyncCommand { get; private set; }
        public AsyncRelayCommand ForceLoadDataAsyncCommand { get; private set; }
        public RelayCommand StartLoggingCommand { get; private set; }
        public AsyncRelayCommand FullAppResetAsyncCommand { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            if (!IsPersistentLoggingEnabled)
            {
                DisposeLogSubscription();
                LogEvents = null;
            }
        }

        private void DisposeLogSubscription()
        {
            var sub = _logSubscription;
            if (sub != null)
            {
                sub.Dispose();
                _logSubscription = null;
            }
        }

        public sealed class ErrorLogInfoWrapper
        {
            public string Name { get; set; }
            public AsyncRelayCommand<string> Command { get; set; }
        }
    }
}
