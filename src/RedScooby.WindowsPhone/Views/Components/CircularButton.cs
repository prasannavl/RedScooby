// Author: Prasanna V. Loganathar
// Created: 9:20 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RedScooby.Views.Components
{
    public class CircularButton : Button
    {
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof (CornerRadius), typeof (CircularButton),
                new PropertyMetadata(new CornerRadius(1000)));

        private static Style _defaultStyle;

        public CircularButton()
        {
            if (_defaultStyle == null)
            {
                _defaultStyle = (Style) Application.Current.Resources["CircularButtonStyle"];
            }
            Style = _defaultStyle;
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
    }
}
