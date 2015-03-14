// Author: Prasanna V. Loganathar
// Created: 10:25 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.Media;
using Windows.Media.Playback;

namespace RedScooby.WindowsPhone.RuntimeComponents
{
    public sealed class BackgroundSirenTask : IBackgroundTask
    {
        private const string sirenPath = "Assets\\audio\\siren.mp4";
        private readonly TimeSpan skipSilenceTimeSpan = TimeSpan.FromMilliseconds(375);
        private BackgroundTaskDeferral deferral;
        private PlaybackMediaMarker endMarker;
        private SystemMediaTransportControls controls;
        private MediaPlayer player;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;
            SetupMediaTransportControls();
            var _ = InitializeBackgroundAudio();
        }

        public void SetupMediaTransportControls()
        {
            controls = SystemMediaTransportControls.GetForCurrentView();

            controls.IsEnabled = true;
            controls.IsPlayEnabled = true;

            // Have the controls disabled until a reliable way to communicate changes is discovered.

            controls.IsPauseEnabled = false;
            controls.IsNextEnabled = false;
            controls.IsPreviousEnabled = false;
            controls.IsChannelUpEnabled = false;
            controls.IsChannelDownEnabled = false;

            controls.DisplayUpdater.Type = MediaPlaybackType.Music;
            controls.DisplayUpdater.MusicProperties.Title = "Siren";
            controls.DisplayUpdater.MusicProperties.Artist = "RedScooby";
            controls.DisplayUpdater.Update();

            controls.ButtonPressed += (sender, args) =>
            {
                switch (args.Button)
                {
                    case SystemMediaTransportControlsButton.Play:
                        player.Play();
                        break;
                    case SystemMediaTransportControlsButton.Pause:
                        player.Pause();
                        break;
                    default:
                        break;
                }
            };
        }

        private async Task InitializeBackgroundAudio()
        {
            player = BackgroundMediaPlayer.Current;
            player.AutoPlay = false;
            player.IsLoopingEnabled = false;

            var t = await Package.Current.InstalledLocation.GetFileAsync(sirenPath);
            var stream = await t.OpenStreamForReadAsync();

            player.SetStreamSource(stream.AsRandomAccessStream());
            player.MediaOpened += (sender, args) =>
            {
                try
                {
                    var marker = new PlaybackMediaMarker(sender.NaturalDuration);
                    endMarker = marker;
                    sender.PlaybackMediaMarkers.Clear();
                    sender.PlaybackMediaMarkers.Insert(marker);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("AudioTask: MediaOpened => " + ex.Message);
                }
            };


            player.PlaybackMediaMarkerReached += (sender, args) =>
            {
                var marker = endMarker;
                if (marker != null && args.PlaybackMediaMarker.Time == marker.Time)
                {
                    try
                    {
                        sender.Pause();
                        sender.Position = skipSilenceTimeSpan;
                        sender.Play();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("AudioTask: MarkerReached => " + ex.Message);
                    }
                }
            };
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            BackgroundMediaPlayer.Shutdown();
            deferral.Complete();
        }
    }
}
