// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Logging.Core
{
    public enum LogLevel
    {
        Trace = LogLevelInternal.Trace,
        Debug = LogLevelInternal.Debug,
        Info = LogLevelInternal.Info,
        Warn = LogLevelInternal.Warn,
        Error = LogLevelInternal.Error,
        Critical = LogLevelInternal.Critical
    }

    [Flags]
    public enum LogLevelInternal : byte
    {
        Off = 0x00,
        Undefined = 0x01,
        Critical = 0x02,
        Error = 0x04,
        Warn = 0x08,
        Info = 0x10,
        Debug = 0x20,
        Trace = 0x40,

        Enabled = 0x80,

        EnabledCriticalLowerThreshold = Enabled | Undefined,
        EnabledErrorLowerThreshold = Enabled | Critical,
        EnabledWarnLowerThreshold = Enabled | Error,
        EnabledInfoLowerThreshold = Enabled | Info,
        EnabledDebugLowerThreshold = Enabled | Info,
        EnabledTraceLowerThreshold = Enabled | Debug
    }
}
