// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using RedScooby.Utilities;

namespace RedScooby.Helpers
{
    public static class StoryboardExtensions
    {
        public static Task RunAsync(this Storyboard storyboard)
        {
            var tcs = new TaskCompletionSource<bool>();
            EventHandler<object> handler = null;
            handler = (sender, o) =>
            {
                var sb = (Storyboard) sender;
                sb.Completed -= handler;
                tcs.SetResult(true);
            };

            storyboard.Completed += handler;
            storyboard.Begin();

            return tcs.Task;
        }

        public static Task RunOnceOrSkipToEndAsync(this Storyboard storyboard, ref bool runOnceCheck)
        {
            if (!runOnceCheck)
            {
                runOnceCheck = true;
                return storyboard.RunAsync();
            }
            storyboard.Begin();
            storyboard.SkipToFill();
            return TaskCache.False;
        }

        public static Task RunReverseAsync(this Storyboard storyboard)
        {
            var tcs = new TaskCompletionSource<bool>();
            var endTime = storyboard.GetCurrentTime();
            var reverseState = storyboard.AutoReverse;

            EventHandler<object> handler = null;
            handler = (sender, o) =>
            {
                var sb = (Storyboard) sender;
                sb.Completed -= handler;
                sb.AutoReverse = reverseState;
                sb.Stop();
                tcs.SetResult(true);
            };

            storyboard.Completed += handler;
            storyboard.AutoReverse = true;
            storyboard.Begin();
            storyboard.Pause();
            storyboard.SeekAlignedToLastTick(endTime);
            storyboard.Resume();
            return tcs.Task;
        }
    }
}
