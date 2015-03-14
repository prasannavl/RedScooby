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
    public static class Text
    {
        public static void ToggleText(UIElement target, string textContent = null, double fontSize = -1,
            Thickness? margin = null, double duration = -1, EventHandler<object> callBack = null)
        {
            double midDuration = 390;
            if (duration >= 0) midDuration = duration/2;

            var s = new Storyboard();

            ExtendedActions.FlippyToggle(target, s, null,
                new List<double> {midDuration - 10, midDuration + 10, midDuration*1.67});

            if (textContent != null)
            {
                Actions.ChangeTextContent(target, s, textContent, midDuration);
            }
            if (fontSize > -1)
            {
                Actions.ShiftFontSize(target, s, fontSize, midDuration);
            }
            if (margin != null)
            {
                Actions.ShiftMargin(target, s, margin.Value, midDuration);
            }

            if (callBack != null)
                s.Completed += callBack;

            s.Begin();
        }

        public static void ToggleTextView(UIElement target, UIElement view, string textContent = null, double width = -1,
            double height = -1, Thickness? margin = null, double duration = -1, EventHandler<object> callBack = null)
        {
            double midDuration = 390;
            if (duration >= 0) midDuration = duration/2;

            var s = new Storyboard();

            ExtendedActions.FlippyToggle(target, s, null,
                new List<double> {midDuration - 10, midDuration + 10, midDuration*1.67});

            if (textContent != null)
            {
                Actions.ChangeTextContent(target, s, textContent, midDuration);
            }
            if (width > -1)
            {
                Actions.ShiftWidth(view, s, width, midDuration);
            }
            if (height > -1)
            {
                Actions.ShiftHeight(view, s, height, midDuration);
            }
            if (margin != null)
            {
                Actions.ShiftMargin(view, s, margin.Value, midDuration);
            }

            if (callBack != null)
                s.Completed += callBack;

            s.Begin();
        }
    }
}
