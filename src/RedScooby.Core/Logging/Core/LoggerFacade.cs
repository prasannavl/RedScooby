// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Logging.Core
{
    public class LoggerFacade : ILoggerControl, ILoggerImpl
    {
        protected readonly LoggerBase Logger;

        public LoggerFacade(LoggerBase logger)
        {
            Logger = logger;
        }

        public void Dispose()
        {
            throw FacadeInvalidOperationException();
        }

        public virtual void Execute(LogLevel level, string message)
        {
            Logger.Execute(level, message);
        }

        void ILogger.Critical(string text)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = Logger.GenerateString(text);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(string text, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = Logger.GenerateString(text, args);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Func<string> textFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = Logger.GenerateString(textFunc());
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Func<object[], string> textFunc, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = Logger.GenerateString(textFunc(args));
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Exception exception)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = Logger.GenerateString(exception);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Func<FormatterDelegate, string> messageFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Func<ExceptionFormatterDelegate, string> messageFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Func<ExceptionFormatterDelegate, Exception, string> messageFunc, Exception ex)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString, ex);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Func<FormatterDelegate, object[], string> messageFunc,
            params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString, args);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Debug(string text)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = Logger.GenerateString(text);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(string text, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = Logger.GenerateString(text, args);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Func<string> textFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = Logger.GenerateString(textFunc());
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Func<object[], string> textFunc, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = Logger.GenerateString(textFunc(args));
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Exception exception)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = Logger.GenerateString(exception);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Func<FormatterDelegate, string> messageFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Func<ExceptionFormatterDelegate, string> messageFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Func<ExceptionFormatterDelegate, Exception, string> messageFunc, Exception ex)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString, ex);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Func<FormatterDelegate, object[], string> messageFunc, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString, args);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Info(string text)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = Logger.GenerateString(text);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(string text, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = Logger.GenerateString(text, args);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Func<string> textFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = Logger.GenerateString(textFunc());
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Func<object[], string> textFunc, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = Logger.GenerateString(textFunc(args));
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Exception exception)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = Logger.GenerateString(exception);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Func<FormatterDelegate, string> messageFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Func<ExceptionFormatterDelegate, string> messageFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Func<ExceptionFormatterDelegate, Exception, string> messageFunc, Exception ex)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString, ex);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Func<FormatterDelegate, object[], string> messageFunc, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString, args);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Warn(string text)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = Logger.GenerateString(text);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(string text, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = Logger.GenerateString(text, args);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Func<string> textFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = Logger.GenerateString(textFunc());
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Func<object[], string> textFunc, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = Logger.GenerateString(textFunc(args));
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Exception exception)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = Logger.GenerateString(exception);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Func<FormatterDelegate, string> messageFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Func<ExceptionFormatterDelegate, string> messageFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Func<ExceptionFormatterDelegate, Exception, string> messageFunc, Exception ex)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString, ex);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Func<FormatterDelegate, object[], string> messageFunc, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString, args);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Trace(string text)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = Logger.GenerateString(text);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(string text, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = Logger.GenerateString(text, args);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Func<string> textFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = Logger.GenerateString(textFunc());
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Func<object[], string> textFunc, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = Logger.GenerateString(textFunc(args));
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Exception exception)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = Logger.GenerateString(exception);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Func<FormatterDelegate, string> messageFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Func<ExceptionFormatterDelegate, string> messageFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Func<ExceptionFormatterDelegate, Exception, string> messageFunc, Exception ex)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString, ex);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Func<FormatterDelegate, object[], string> messageFunc, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString, args);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Error(string text)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = Logger.GenerateString(text);
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(string text, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = Logger.GenerateString(text, args);
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Func<string> textFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = Logger.GenerateString(textFunc());
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Func<object[], string> textFunc, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = Logger.GenerateString(textFunc(args));
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Exception exception)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = Logger.GenerateString(exception);
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Func<FormatterDelegate, string> messageFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString);
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Func<ExceptionFormatterDelegate, string> messageFunc)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString);
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Func<ExceptionFormatterDelegate, Exception, string> messageFunc, Exception ex)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString, ex);
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Func<FormatterDelegate, object[], string> messageFunc, params object[] args)
        {
            if (Logger.LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = messageFunc(Logger.GenerateDestructuredString, args);
                Execute(LogLevel.Error, message);
            }
        }

        public void Enable()
        {
            throw FacadeInvalidOperationException();
        }

        public void Disable()
        {
            throw FacadeInvalidOperationException();
        }

        public LogLevel GetLogLevel()
        {
            return Logger.GetLogLevel();
        }

        public void SetLogLevel(LogLevel level)
        {
            throw FacadeInvalidOperationException();
        }

        public bool IsEnabled
        {
            get { return Logger.IsEnabled; }
        }

        public bool IsSynchronized
        {
            get { return Logger.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return Logger.SyncRoot; }
        }

        private InvalidOperationException FacadeInvalidOperationException()
        {
            return
                new InvalidOperationException(
                    "This operation cannot be done on a facade. Perform it on the actual logger");
        }
    }
}
