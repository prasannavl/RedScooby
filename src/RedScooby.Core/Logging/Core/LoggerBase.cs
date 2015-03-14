// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Globalization;

namespace RedScooby.Logging.Core
{
    public abstract class LoggerBase : LoggerControl, ILoggerImpl
    {
        protected LoggerBase()
        {
            FormatProvider = CultureInfo.InvariantCulture;
        }

        protected LoggerBase(IFormatProvider formatProvider)
        {
            if (formatProvider == null) throw new ArgumentNullException("formatProvider");

            FormatProvider = formatProvider;
        }

        protected IFormatProvider FormatProvider { get; set; }
        public abstract void Execute(LogLevel level, string message);

        void ILogger.Critical(string text)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = GenerateString(text);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(string text, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = GenerateString(text, args);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Func<string> textFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = GenerateString(textFunc());
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Func<object[], string> textFunc, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = GenerateString(textFunc(args));
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Exception exception)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = GenerateString(exception);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Func<FormatterDelegate, string> messageFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Func<ExceptionFormatterDelegate, string> messageFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Func<ExceptionFormatterDelegate, Exception, string> messageFunc, Exception ex)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString, ex);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Critical(Func<FormatterDelegate, object[], string> messageFunc,
            params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledCriticalLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString, args);
                Execute(LogLevel.Critical, message);
            }
        }

        void ILogger.Debug(string text)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = GenerateString(text);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(string text, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = GenerateString(text, args);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Func<string> textFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = GenerateString(textFunc());
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Func<object[], string> textFunc, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = GenerateString(textFunc(args));
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Exception exception)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = GenerateString(exception);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Func<FormatterDelegate, string> messageFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Func<ExceptionFormatterDelegate, string> messageFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Func<ExceptionFormatterDelegate, Exception, string> messageFunc, Exception ex)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString, ex);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Debug(Func<FormatterDelegate, object[], string> messageFunc, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledDebugLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString, args);
                Execute(LogLevel.Debug, message);
            }
        }

        void ILogger.Info(string text)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = GenerateString(text);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(string text, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = GenerateString(text, args);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Func<string> textFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = GenerateString(textFunc());
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Func<object[], string> textFunc, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = GenerateString(textFunc(args));
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Exception exception)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = GenerateString(exception);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Func<FormatterDelegate, string> messageFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Func<ExceptionFormatterDelegate, string> messageFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Func<ExceptionFormatterDelegate, Exception, string> messageFunc, Exception ex)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString, ex);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Info(Func<FormatterDelegate, object[], string> messageFunc, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledInfoLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString, args);
                Execute(LogLevel.Info, message);
            }
        }

        void ILogger.Warn(string text)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = GenerateString(text);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(string text, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = GenerateString(text, args);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Func<string> textFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = GenerateString(textFunc());
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Func<object[], string> textFunc, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = GenerateString(textFunc(args));
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Exception exception)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = GenerateString(exception);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Func<FormatterDelegate, string> messageFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Func<ExceptionFormatterDelegate, string> messageFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Func<ExceptionFormatterDelegate, Exception, string> messageFunc, Exception ex)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString, ex);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Warn(Func<FormatterDelegate, object[], string> messageFunc, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledWarnLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString, args);
                Execute(LogLevel.Warn, message);
            }
        }

        void ILogger.Trace(string text)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = GenerateString(text);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(string text, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = GenerateString(text, args);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Func<string> textFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = GenerateString(textFunc());
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Func<object[], string> textFunc, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = GenerateString(textFunc(args));
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Exception exception)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = GenerateString(exception);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Func<FormatterDelegate, string> messageFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Func<ExceptionFormatterDelegate, string> messageFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Func<ExceptionFormatterDelegate, Exception, string> messageFunc, Exception ex)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString, ex);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Trace(Func<FormatterDelegate, object[], string> messageFunc, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledTraceLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString, args);
                Execute(LogLevel.Trace, message);
            }
        }

        void ILogger.Error(string text)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = GenerateString(text);
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(string text, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = GenerateString(text, args);
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Func<string> textFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = GenerateString(textFunc());
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Func<object[], string> textFunc, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = GenerateString(textFunc(args));
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Exception exception)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = GenerateString(exception);
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Func<FormatterDelegate, string> messageFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString);
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Func<ExceptionFormatterDelegate, string> messageFunc)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString);
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Func<ExceptionFormatterDelegate, Exception, string> messageFunc, Exception ex)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString, ex);
                Execute(LogLevel.Error, message);
            }
        }

        void ILogger.Error(Func<FormatterDelegate, object[], string> messageFunc, params object[] args)
        {
            if (LogLevelInternal > LogLevelInternal.EnabledErrorLowerThreshold)
            {
                var message = messageFunc(GenerateDestructuredString, args);
                Execute(LogLevel.Error, message);
            }
        }

        protected internal virtual string GenerateDestructuredString(string text, params object[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            if (text == null) text = "{0}";

            return string.Format(FormatProvider, text, args);
        }

        protected internal virtual string GenerateDestructuredString(Exception exception)
        {
            return string.Format(FormatProvider, "{0}", exception);
        }

        protected internal virtual string GenerateString(string text)
        {
            return text;
        }

        protected internal virtual string GenerateString(string text, params object[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            if (text == null) text = "{0}";

            return string.Format(CultureInfo.InvariantCulture, text, args);
        }

        protected internal virtual string GenerateString(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException("exception");

            return exception.Message;
        }
    }
}
