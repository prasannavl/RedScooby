// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using RedScooby.Actions;
using RedScooby.Common;
using RedScooby.Components;
using RedScooby.Helpers;
using RedScooby.ViewModels.Assistance;
using RedScooby.Views.Components;

namespace RedScooby.Views.Assistance
{
    public class DistressActiveViewBase : UserControlView<DistressActiveViewModel> { }

    public partial class DistressActiveView
    {
        public readonly SolidColorBrush RegularForeground = new SolidColorBrush(Colors.Black);
        public readonly SolidColorBrush SelectedForeground = new SolidColorBrush(Colors.OrangeRed);

        public DistressActiveView()
        {
            InitializeComponent();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            SubscribeTo(ViewActions.Distress.CategoryRequery, () =>
            {
                SetCategoryState(DefenseButton, Model.GetCategoryState(DistressCategory.Defence));
                SetCategoryState(MedicalButton, Model.GetCategoryState(DistressCategory.Medical));
                SetCategoryState(AccicentButton, Model.GetCategoryState(DistressCategory.Accident));
            });
        }

        private void SetCategoryState(PulsingCircularButton button, ActivityState categoryState)
        {
            button.SetState(categoryState);
        }

        // Flyout options don't support data binding. So manually update model for now

        private void UpdateNotificationPreference(object sender, RoutedEventArgs e)
        {
            var flyoutItem = ((MenuFlyoutItem) sender);
            var option = (DistressNotificationOptionDetail) flyoutItem.Tag;
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

                var tag = (DistressNotificationOptionDetail) flyout.Tag;
                if (tag != null)
                {
                    if (tag.OptionTitle == option.OptionTitle)
                    {
                        if (x.Foreground != SelectedForeground)
                            x.Foreground = SelectedForeground;
                    }
                    else
                    {
                        if (x.Foreground != RegularForeground)
                            x.Foreground = RegularForeground;
                    }
                }
            });
        }

        private void ToggleCategory(object sender, RoutedEventArgs e)
        {
            var category = DistressCategory.Defence;
            if (sender == DefenseButton)
            {
                category = DistressCategory.Defence;
            }
            else if (sender == MedicalButton)
            {
                category = DistressCategory.Medical;
            }
            else if (sender == AccicentButton)
            {
                category = DistressCategory.Accident;
            }

            Model.ToggleCategory(category);
        }

        private void NotificationTargetsFlyout_OnOpened(object sender, object e)
        {
            UpdateNotificationPreferenceFromModel((MenuFlyout) sender);
        }
    }
}
