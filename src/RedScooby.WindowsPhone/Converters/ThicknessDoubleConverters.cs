// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RedScooby.Converters
{
    public class DoubleToThicknessConverter : IValueConverter
    {
        private readonly ThicknessToDoubleConverter reverseConverter;

        public DoubleToThicknessConverter()
        {
            reverseConverter = new ThicknessToDoubleConverter();
        }

        public DoubleToThicknessConverter(double offset)
        {
            reverseConverter = new ThicknessToDoubleConverter(offset);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return reverseConverter.ConvertBack(value, targetType, parameter, language);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return reverseConverter.Convert(value, targetType, parameter, language);
        }
    }

    public class ThicknessToDoubleConverter : IValueConverter
    {
        private readonly double offset;
        public ThicknessToDoubleConverter() { }

        public ThicknessToDoubleConverter(double offset)
        {
            this.offset = offset;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var res = (Thickness) value;
            switch ((string) parameter)
            {
                case "Top":
                    return res.Top;
                case "Bottom":
                    return res.Bottom;
                case "Left":
                    return res.Left;
                case "Right":
                    return res.Right;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var val = (double) value;
            switch ((string) parameter)
            {
                case "Top":
                    return new Thickness(0, val, 0, 0);
                case "Bottom":
                    return new Thickness(0, 0, 0, val);
                case "Left":
                    return new Thickness(val, 0, 0, 0);
                case "Right":
                    return new Thickness(0, 0, val, 0);
            }

            return new Thickness(0, 0, 0, 0);
        }
    }
}
