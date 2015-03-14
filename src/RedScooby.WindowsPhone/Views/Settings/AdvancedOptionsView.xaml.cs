// Author: Prasanna V. Loganathar
// Created: 9:23 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using RedScooby.Helpers;
using RedScooby.ViewModels;
using RedScooby.Views.Components;

namespace RedScooby.Views.Settings
{
    public class AdvancedOptionsViewBase : PageView<WinRtAdvancedOptionsViewModel> { }

    public sealed partial class AdvancedOptionsView
    {
        public AdvancedOptionsView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            Model.StartLoggingCommand.Execute(null);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            WindowHelpers.GoBack();
        }
    }
}
