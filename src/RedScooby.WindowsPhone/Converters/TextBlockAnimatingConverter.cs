// Author: Prasanna V. Loganathar
// Created: 4:10 AM 13-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using RedScooby.Animations;

namespace RedScooby.Converters
{
    public class TextBlockAnimatingConverter : IValueConverter
    {
        public TextBlock CurrentTextBlock { get; set; }
        public bool DontAnimateOne { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var newText = (string) value;
            var sender = CurrentTextBlock;

            if (DontAnimateOne)
            {
                DontAnimateOne = false;
                sender = null;
            }
            if (sender == null) return newText;

            var _ = Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Low,
                () => { Text.ToggleText(sender, newText); });

            return sender.Text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
