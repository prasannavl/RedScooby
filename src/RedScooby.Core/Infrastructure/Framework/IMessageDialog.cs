// Author: Prasanna V. Loganathar
// Created: 12:00 AM 19-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

namespace RedScooby.Infrastructure.Framework
{
    public interface IMessageDialogHelper
    {
        void Show(string message, string title = null, string dismissText = null);
    }
}
