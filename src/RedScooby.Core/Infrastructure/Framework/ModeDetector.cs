// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Windows.ApplicationModel;

namespace RedScooby.Infrastructure.Framework
{
    public static class ModeDetector
    {
        static ModeDetector()
        {
#if NETFX_CORE
            IsInDesignMode = DesignMode.DesignModeEnabled;
#endif
        }

        public static bool IsInDebugMode
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsInDesignMode { get; set; }
    }
}
