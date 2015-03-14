// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.ViewModels.Assistance;
using RedScooby.Views.Components;

namespace RedScooby.Views.Assistance
{
    public class DistressCountdownViewBase : UserControlView<DistressCountdownViewModel> { }

    public partial class DistressCountdownView
    {
        public DistressCountdownView()
        {
            InitializeComponent();
        }
    }
}
