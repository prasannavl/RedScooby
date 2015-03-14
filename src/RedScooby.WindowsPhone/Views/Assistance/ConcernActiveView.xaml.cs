// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using RedScooby.Actions;
using RedScooby.Converters;
using RedScooby.Helpers;
using RedScooby.ViewModels.Assistance;
using RedScooby.Views.Components;

namespace RedScooby.Views.Assistance
{
    public class ConcernActiveViewBase : UserControlView<ConcernActiveViewModel> { }

    public partial class ConcernActiveView
    {
        public readonly SolidColorBrush RegularForeground = new SolidColorBrush(Colors.Black);
        public readonly SolidColorBrush SelectedForeground = new SolidColorBrush(Colors.OrangeRed);
        private readonly TextBlockAnimatingConverter textBlockAnimator;

        public ConcernActiveView()
        {
            InitializeComponent();
            textBlockAnimator = (TextBlockAnimatingConverter) Resources["TextBlockAnimatingConverter"];
            textBlockAnimator.CurrentTextBlock = ConcernModeInfoTextBlock;
            textBlockAnimator.DontAnimateOne = true;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            SubscribeTo(ViewActions.PinFeedback.Show, () => { Model.SupressChangeNotifications(); });
            SubscribeTo(ViewActions.PinFeedback.Hide, () =>
            {
                textBlockAnimator.DontAnimateOne = true;
                Model.RefreshChangeNotifications();
                Model.EnableChangeNotifications();
            });
        }

        // Flyout options don't support data binding. So manually update model for now

        private void UpdateNotificationPreference(object sender, RoutedEventArgs e)
        {
            var flyoutItem = ((MenuFlyoutItem) sender);
            var option = (ConcernNotificationOptionDetail) flyoutItem.Tag;
            Model.NotificationPreference = option;
        }

        // Flyout options don't support data binding. So manually update model for now
        private void UpdateNotificationPreferenceFromModel(MenuFlyout notificationTargetsFlyout)
        {
            var option = Model.NotificationPreference;

            notificationTargetsFlyout.Items.ForEach(x =>
            {
                var flyout = (MenuFlyoutItem) x;
                flyout.Click += UpdateNotificationPreference;

                var tag = (ConcernNotificationOptionDetail) flyout.Tag;
                if (tag != null)
                {
                    if (tag.OptionTitle == option.OptionTitle)
                    {
                        if (flyout.Foreground != SelectedForeground)
                        {
                            flyout.Foreground = SelectedForeground;
                        }
                    }
                    else
                    {
                        if (flyout.Foreground != RegularForeground)
                        {
                            flyout.Foreground = RegularForeground;
                        }
                    }
                }
            });
        }

        private void NotificationTargetsFlyout_OnOpened(object sender, object e)
        {
            UpdateNotificationPreferenceFromModel((MenuFlyout) sender);
        }

        private void TimerTextBlock_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Model.HalveFeedbackTimeCommand.Execute(null);
        }

        private void TimerFeedbackRenew_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var _ = Model.RequestImmediateFeedbackAsyncCommand.ExecuteAsync();
        }
    }
}
