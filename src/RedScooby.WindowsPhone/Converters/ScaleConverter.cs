// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Data;
using RedScooby.Logging;

namespace RedScooby.Converters
{
    public class ScaleConverter : IValueConverter
    {
        private double scaleFactor = 1;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double result;
            if (Double.TryParse((string) parameter, out result))
            {
                var scaledSize = scaleFactor*result;
                return scaledSize;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        // Empty ctor to be instantiated in XAML

        public void Initialize(DisplayInformation info)
        {
            if (info == null) throw new ArgumentNullException("info");

            Log.Trace(f => f("Display Info: {0}", info));
            var px = info.RawPixelsPerViewPixel;

            if (px < 1.1)
            {
                scaleFactor = 1.2;
            }
            else if (px >= 1.1 && px < 1.3)
            {
                scaleFactor = 1.8;
            }
            else if (px >= 1.3 && px < 1.5)
            {
                scaleFactor = 1.8;
            }
            else if (px >= 1.5 && px < 1.7)
            {
                scaleFactor = 1.8;
            }
            else if (px >= 1.7 && px < 1.9)
            {
                scaleFactor = 1.8;
            }
            else if (px >= 1.9 && px < 2.1)
            {
                scaleFactor = 1.8;
            }
            else if (px >= 2.1 && px < 2.3)
            {
                scaleFactor = 2.2;
            }
            else
            {
                scaleFactor = 2.2;
            }

            Log.Info("Scaling factor: " + scaleFactor);
        }
    }
}
