// Author: Prasanna V. Loganathar
// Created: 3:49 PM 23-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

namespace RedScooby.ViewModels
{
    public class WinRtViewModelLocator
    {
        public WinRtSettingsViewModel SettingsViewModel
        {
            get { return ViewModelLocator.Scope.Locate<WinRtSettingsViewModel>(); }
        }

        public WinRtAdvancedOptionsViewModel AdvancedOptionsViewModel
        {
            get { return ViewModelLocator.Scope.Locate<WinRtAdvancedOptionsViewModel>(); }
        }
    }
}
