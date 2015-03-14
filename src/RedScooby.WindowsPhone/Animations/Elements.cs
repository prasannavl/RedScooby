// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace RedScooby.Animations
{
    public static class Elements
    {
        public static void Appear(UIElement obj, double duration = 300, EventHandler<object> callBack = null)
        {
            var s = new Storyboard();

            Actions.OpacityShift(obj, s, new List<double> {1}, new List<double> {duration});

            if (callBack != null)
                s.Completed += callBack;

            s.Begin();
        }

        public static void Disappear(UIElement obj, double duration = 300, EventHandler<object> callBack = null)
        {
            var s = new Storyboard();

            Actions.OpacityShift(obj, s, new List<double> {0}, new List<double> {duration});

            if (callBack != null)
                s.Completed += callBack;

            s.Begin();
        }

        public static void FlipAndHide(UIElement target, double duration = 300, EventHandler<object> callBack = null)
        {
            var s = new Storyboard();
            var durationList = new List<double> {duration};
            Actions.FlipY(target, s, new List<double> {0}, durationList);
            Actions.OpacityShift(target, s, new List<double> {0}, durationList);
            if (callBack != null)
                s.Completed += callBack;
            s.Begin();
        }

        public static void FlipAndShow(UIElement target, double duration = 300, EventHandler<object> callBack = null)
        {
            var s = new Storyboard();
            var durationList = new List<double> {0, duration};
            Actions.FlipY(target, s, new List<double> {0, 1}, durationList);
            Actions.OpacityShift(target, s, new List<double> {0, 1}, durationList);
            if (callBack != null)
                s.Completed += callBack;
            s.Begin();
        }

        public static void VerticalFlip(UIElement obj, double duration = 300, EventHandler<object> callBack = null)
        {
            var s = new Storyboard();

            Actions.FlipY(obj, s, new List<double> {-1}, new List<double> {duration});

            if (callBack != null)
                s.Completed += callBack;

            s.Begin();
        }
    }
}
