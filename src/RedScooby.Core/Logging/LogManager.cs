// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using RedScooby.Logging.Core;

namespace RedScooby.Logging
{
    public class LogManager
    {
        public static IObservable<LogEvent> Events
        {
            get { return Log.Logger.Events; }
        }

        public static void Disable()
        {
            Log.Logger.Disable();
        }

        public static void Enable()
        {
            Log.Logger.Enable();
        }

        public static void SetLevel(LogLevel level)
        {
            Log.Logger.SetLogLevel(level);
        }

        public static void SetLogger(IObservableLogger logger)
        {
            var currentControl = Log.Logger;
            Log.Logger = logger;
            currentControl.Dispose();
        }
    }
}
