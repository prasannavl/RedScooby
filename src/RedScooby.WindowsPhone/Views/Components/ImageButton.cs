// Author: Prasanna V. Loganathar
// Created: 9:20 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace RedScooby.Views.Components
{
    public sealed class ImageButton : Button
    {
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource",
                typeof (ImageSource),
                typeof (ImageButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text",
                typeof (string),
                typeof (ImageButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof (double), typeof (ImageButton), new PropertyMetadata(50.0));

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof (double), typeof (ImageButton), new PropertyMetadata(50.0));

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof (Thickness), typeof (ImageButton),
                new PropertyMetadata(new Thickness(0, 10, 0, 0)));

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof (TextAlignment), typeof (ImageButton),
                new PropertyMetadata(TextAlignment.Center));

        public static readonly DependencyProperty ImageStretchOptionsProperty =
            DependencyProperty.Register("ImageStretchOptions", typeof (Stretch), typeof (ImageButton),
                new PropertyMetadata(Stretch.UniformToFill));

        private static Style _defaultStyle;

        public ImageButton()
        {
            if (_defaultStyle == null)
            {
                _defaultStyle = (Style) Application.Current.Resources["ImageButtonStyle"];
            }
            Style = _defaultStyle;
            DataContext = this;
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource) GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public double ImageSizeOverride
        {
            get
            {
                if (Math.Abs(ImageHeight - ImageWidth) < 0)
                    return ImageHeight;

                return -1;
            }
            set
            {
                if (value > -1)
                {
                    SetValue(ImageHeightProperty, value);
                    SetValue(ImageWidthProperty, value);
                }
            }
        }

        public double SizeOverride
        {
            get
            {
                if (Math.Abs(Height - Width) < 0)
                    return Height;

                return -1;
            }
            set
            {
                if (value > -1)
                {
                    SetValue(HeightProperty, value);
                    SetValue(WidthProperty, value);
                }
            }
        }

        public double ImageHeight
        {
            get { return (double) GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public double ImageWidth
        {
            get { return (double) GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public Thickness TextMargin
        {
            get { return (Thickness) GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        public double TextFontSize
        {
            get { return (double) GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment) GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public Stretch ImageStretchOptions
        {
            get { return (Stretch) GetValue(ImageStretchOptionsProperty); }
            set { SetValue(ImageStretchOptionsProperty, value); }
        }
    }
}
