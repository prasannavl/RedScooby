// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace RedScooby.Animations
{
    public static class ExtendedActions
    {
        public static void FlippyToggle(UIElement obj, Storyboard s, IEnumerable<double> valueCollection = null,
            IEnumerable<double> durationCollection = null)
        {
            Actions.OpacityShift(obj, s, valueCollection, durationCollection);
            Actions.FlipY(obj, s, valueCollection, durationCollection);
        }
    }
}
