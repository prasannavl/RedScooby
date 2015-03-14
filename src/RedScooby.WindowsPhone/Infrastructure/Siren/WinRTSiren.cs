// Author: Prasanna V. Loganathar
// Created: 6:48 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Threading.Tasks;
using Windows.Media.Playback;
using RedScooby.Utilities;

namespace RedScooby.Infrastructure.Siren
{
    public class WinRtSiren : ISirenService
    {
        public void Dispose() { }

        public Task TurnOffAsync()
        {
            BackgroundMediaPlayer.Current.Pause();
            return TaskCache.Completed;
        }

        public Task TurnOnAsync()
        {
            BackgroundMediaPlayer.Current.Play();
            return TaskCache.Completed;
        }

        public bool IsOn
        {
            get
            {
                var player = BackgroundMediaPlayer.Current;
                var state = player.CurrentState;
                var pos = player.Position;
                return (state == MediaPlayerState.Playing ||
                        (state == MediaPlayerState.Paused && player.Position != pos));
            }
        }
    }
}
