// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Windows.UI.Xaml.Controls;

namespace RedScooby.Helpers
{
    public static class ControlExtensions
    {
        public static void MoveCaretToEnd(this TextBox textbox)
        {
            if (textbox != null && textbox.Text != null && textbox.Text.Length > 0)
                textbox.Select(textbox.Text.Length, textbox.Text.Length);
        }
    }
}
