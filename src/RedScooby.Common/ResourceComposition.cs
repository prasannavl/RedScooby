// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Common.Resources;

namespace RedScooby.Common
{
    /// <summary>
    ///     This project is essentially just a portable way to carry around the text resources, instead of recreating them.
    ///     ResX files are required for PCL. While the Windows Phone projects only support ResW format.
    ///     So, separate PCL which holds all the ResX.
    /// </summary>
    public class ResourceComposition
    {
        public ResourceComposition()
        {
            CommonStrings = new CommonStrings();
            MenuStrings = new MenuStrings();
            FormattedStrings = new FormattedStrings();
            TabStrings = new TabStrings();
        }

        public CommonStrings CommonStrings { get; private set; }
        public TabStrings TabStrings { get; private set; }
        public FormattedStrings FormattedStrings { get; private set; }
        public MenuStrings MenuStrings { get; private set; }
    }
}
