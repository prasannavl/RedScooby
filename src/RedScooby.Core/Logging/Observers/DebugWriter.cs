// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Diagnostics;
using RedScooby.Logging.Core;

namespace RedScooby.Logging.Observers
{
    public class DebugWriter : IObserver<LogEvent>
    {
        public void OnCompleted() { }

        public void OnError(Exception error)
        {
            Debug.WriteLine("{0} [Internal error]: {1}", DateTimeOffset.Now.ToString("s"), error.Message);
            Debug.WriteLine("    [Trace] : " + error.StackTrace);
        }

        public void OnNext(LogEvent value)
        {
            Debug.WriteLine("{0} [{1}]: {2}", value.Timestamp.ToString("s"), value.Level, value.Message);
        }
    }
}
