// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using RedScooby.ViewModels.Assistance;
using RedScooby.Views.Components;

namespace RedScooby.Views.Assistance
{
    public class PinFeedbackViewBase : UserControlView<PinFeedbackViewModel> { }

    public sealed partial class PinFeedbackView
    {
        public readonly SolidColorBrush RegularForeground = new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));
        public readonly SolidColorBrush ErrorForeground = new SolidColorBrush(Color.FromArgb(255, 255, 99, 71));

        public PinFeedbackView()
        {
            InitializeComponent();
        }

        private void OnNumericKey(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var content = (string) button.Content;
            Model.AddToInput(content);
            PinCodeBlock.Foreground = Model.IsPinInvalid ? ErrorForeground : RegularForeground;
        }
    }
}
