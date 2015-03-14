// Author: Prasanna V. Loganathar
// Created: 12:52 AM 19-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Helpers;

namespace RedScooby.Infrastructure.Framework
{
    public class WinRtMessageDialogHelper : IMessageDialogHelper
    {
        public void Show(string message, string title = null, string dismissText = null)
        {
            WindowHelpers.ShowMessageDialog(message, title, dismissText)
                .ContinueWithLogErrorOnException(true);
        }
    }
}
