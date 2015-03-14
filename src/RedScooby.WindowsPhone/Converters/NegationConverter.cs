// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using Windows.UI.Xaml.Data;

namespace RedScooby.Converters
{
    public class NegationConverter : IValueConverter
    {
        public NegationConverter() { }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var type = value.GetType();
            if (type == typeof (int))
            {
                var res = (int) value;
                return -res;
            }
            else if (type == typeof (double))
            {
                var res = (double) value;
                return -res;
            }
            else if (type == typeof (long))
            {
                var res = (long) value;
                return -res;
            }
            else
            {
                throw new InvalidOperationException("Unsupported type");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, targetType, parameter, language);
        }
    }
}
