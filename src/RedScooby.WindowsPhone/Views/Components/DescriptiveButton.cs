// Author: Prasanna V. Loganathar
// Created: 9:20 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RedScooby.Views.Components
{
    public sealed class DescriptiveButton : Button
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title",
            typeof (string), typeof (DescriptiveButton), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register("TitleTemplate",
            typeof (DataTemplate), typeof (DescriptiveButton), new PropertyMetadata(null));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public DataTemplate TitleTemplate
        {
            get { return (DataTemplate) GetValue(TitleTemplateProperty); }
            set { SetValue(TitleTemplateProperty, value); }
        }
    }
}
