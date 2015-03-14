// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using Windows.UI.Xaml.Data;

namespace RedScooby.Converters
{
    public class BooleanToStringConverter : IValueConverter
    {
        private readonly string trueString = "On";
        private readonly string falseString = "Off";
        public BooleanToStringConverter() { }

        public BooleanToStringConverter(string trueString, string falseString)
        {
            this.trueString = trueString;
            this.falseString = falseString;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var res = (bool) value;
            return res ? trueString : falseString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var res = (string) value;
            return res == trueString;
        }
    }
}
