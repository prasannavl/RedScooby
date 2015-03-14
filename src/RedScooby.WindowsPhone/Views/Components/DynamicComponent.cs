// Author: Prasanna V. Loganathar
// Created: 9:20 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using Windows.UI.Xaml;
using RedScooby.Infrastructure.Composition;

namespace RedScooby.Views.Components
{
    public class DynamicComponent : ContentControlView<IViewModel>
    {
        public static DependencyProperty ComponentTypeProperty = DependencyProperty.Register("ComponentType",
            typeof (Type), typeof (DynamicComponent), new PropertyMetadata(null));

        public DynamicComponent()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
        }

        public Type ComponentType
        {
            get { return (Type) GetValue(ComponentTypeProperty); }
            set { SetValue(ComponentTypeProperty, value); }
        }

        public string ComponentTypeString
        {
            set
            {
                var type = Type.GetType(value);
                ComponentType = type;
                DesignComponent.InitializeComponent(() => LoadContent());
            }
            get
            {
                var type = ComponentType;
                return type != null ? type.FullName : null;
            }
        }

        public bool IsLoaded
        {
            get { return Content != null; }
        }

        public void LoadContent(bool isVisible = true, double startOpacity = -1)
        {
            var type = ComponentType;
            if (type != null && Content == null)
            {
                var content = Activator.CreateInstance(ComponentType);
                // Ensure race conditions are handled, but leniently, since its unlikely and is harmless anyway
                if (Content == null)
                {
                    Content = content;
                    if (startOpacity >= 0)
                    {
                        Opacity = startOpacity;
                    }
                    if (isVisible)
                    {
                        Visibility = Visibility.Visible;
                    }
                }
            }
        }

        public void UnloadContent(bool collapseVisibility = true)
        {
            DisposeIfRequired();

            Content = null;
            if (collapseVisibility)
                Visibility = Visibility.Collapsed;
        }

        private void DisposeIfRequired()
        {
            var disp = Content as IDisposable;
            if (disp != null) disp.Dispose();
        }
    }
}
