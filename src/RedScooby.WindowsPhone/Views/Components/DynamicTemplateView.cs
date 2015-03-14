// Author: Prasanna V. Loganathar
// Created: 9:20 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Windows.UI.Xaml;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework;

namespace RedScooby.Views.Components
{
    public class DynamicTemplateView : ContentControlView<IViewModel>
    {
        // Using a DependencyProperty as the backing store for InactiveTemplate. This enables animation, styling, binding.
        public static readonly DependencyProperty InactiveTemplateProperty =
            DependencyProperty.Register("InactiveTemplate", typeof (DataTemplate), typeof (DynamicTemplateView),
                new PropertyMetadata(default(DataTemplate)));

        public static DataTemplate EmptyDataTemplate = new DataTemplate();

        public static DependencyProperty ActiveTemplateProperty = DependencyProperty.Register("ActiveTemplate",
            typeof (DataTemplate), typeof (DynamicTemplateView), new PropertyMetadata(default(DataTemplate)));

        private static readonly object _contentObjectForActiveTemplate = new object();
        private static readonly object _contentObjectForInactiveTemplate = new object();

        public DynamicTemplateView()
        {
            ContentTemplate = EmptyDataTemplate;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
        }

        public DataTemplate ActiveTemplate
        {
            get { return (DataTemplate) GetValue(ActiveTemplateProperty); }
            set
            {
                SetValue(ActiveTemplateProperty, value);
                if (IsActive || ModeDetector.IsInDesignMode)
                {
                    ActivateInternal(Visibility == Visibility.Visible, -1);
                }
            }
        }

        public DataTemplate InactiveTemplate
        {
            get { return (DataTemplate) GetValue(InactiveTemplateProperty); }
            set
            {
                SetValue(InactiveTemplateProperty, value);
                if (!IsActive)
                {
                    DeactivateInternal(Visibility == Visibility.Collapsed, -1);
                }
            }
        }

        public bool IsActive { get; private set; }

        public void Activate(bool isVisible = true, double setOpacity = -1)
        {
            ActivateInternal(isVisible, setOpacity);
        }

        public void Deactivate(bool collapseVisibility = true, double setOpacity = -1)
        {
            DeactivateInternal(collapseVisibility, setOpacity);
        }

        private void ActivateInternal(bool isVisible, double setOpacity)
        {
            if (!IsActive)
            {
                IsActive = true;

                ContentTemplate = ActiveTemplate;
                Content = _contentObjectForActiveTemplate;
            }


            if (isVisible)
                Visibility = Visibility.Visible;

            if (setOpacity > -1)
                Opacity = setOpacity;
        }

        private void DeactivateInternal(bool collapseVisibility, double setOpacity)
        {
            if (IsActive)
            {
                IsActive = false;

                ContentTemplate = InactiveTemplate ?? EmptyDataTemplate;
                Content = _contentObjectForInactiveTemplate;
            }

            if (collapseVisibility)
                Visibility = Visibility.Collapsed;

            if (setOpacity > -1)
                Opacity = setOpacity;
        }
    }
}
