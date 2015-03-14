// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Logging.Core
{
    public interface ILoggerControl : IDisposable
    {
        bool IsEnabled { get; }
        bool IsSynchronized { get; }
        object SyncRoot { get; }
        void Disable();
        void Enable();
        LogLevel GetLogLevel();
        void SetLogLevel(LogLevel level);
    }

    public class LoggerControl : ILoggerControl
    {
#if DEBUG
        private LogLevelInternal logLevelInternal = LogLevelInternal.Debug | LogLevelInternal.Enabled;
#else
        private LogLevelInternal logLevelInternal = LogLevelInternal.Error | LogLevelInternal.Enabled;
#endif

        protected internal LogLevelInternal LogLevelInternal
        {
            get { return logLevelInternal; }
            set { logLevelInternal = value; }
        }

        public virtual bool IsEnabled
        {
            get { return LogLevelInternal > LogLevelInternal.Enabled; }
        }

        public virtual void Enable()
        {
            LogLevelInternal |= LogLevelInternal.Enabled;
        }

        public virtual void Disable()
        {
            LogLevelInternal &= ~LogLevelInternal.Enabled;
        }

        public virtual bool IsSynchronized { get; protected set; }

        public virtual object SyncRoot
        {
            get { throw new NotImplementedException("SyncRoot"); }

            set { throw new NotImplementedException("SyncRoot"); }
        }

        public virtual void Dispose() { }

        public virtual LogLevel GetLogLevel()
        {
            // Clear the enabled flag to get the actual LogLevel.
            return (LogLevel) (LogLevelInternal & ~LogLevelInternal.Enabled);
        }

        public virtual void SetLogLevel(LogLevel level)
        {
            if (!Enum.IsDefined(typeof (LogLevel), level)) throw new ArgumentOutOfRangeException("level");

            var internalLevel = (LogLevelInternal) level;

            // Make sure, the previous state of IsEnabled is restored, 
            // while restoring the log level.
            if (IsEnabled)
            {
                LogLevelInternal = internalLevel | LogLevelInternal.Enabled;
            }
            else
            {
                LogLevelInternal = internalLevel;
            }
        }
    }
}
