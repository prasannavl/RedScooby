// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using RedScooby.Logging.Core;

namespace RedScooby.Logging
{
    public static class Log
    {
        private static IObservableLogger _logger = new NullLogger();
        private static LoggerBase _loggerBase;

        internal static IObservableLogger Logger
        {
            get { return _logger; }
            set
            {
                _logger = value ?? new NullLogger();
                _loggerBase = _logger as LoggerBase;
            }
        }

        public static void Critical(Func<string> textFunc)
        {
            Logger.Critical(textFunc);
        }

        public static void Critical(Exception exception)
        {
            Logger.Critical(exception);
        }

        public static void Critical(Func<FormatterDelegate, string> formatterFunc)
        {
            Logger.Critical(formatterFunc);
        }

        public static void Critical(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            Logger.Critical(formatterFunc);
        }

        public static void Critical(string text)
        {
            Logger.Critical(text);
        }

        public static void Critical(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            Logger.Critical(formatterFunc, ex);
        }

        public static void Critical(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            Logger.Critical(formatterFunc, args);
        }

        public static void Critical(Func<object[], string> textFunc, params object[] args)
        {
            Logger.Critical(textFunc, args);
        }

        public static void Critical(string text, params object[] args)
        {
            Logger.Critical(text, args);
        }

        public static void Debug(Func<string> textFunc)
        {
            Logger.Debug(textFunc);
        }

        public static void Debug(Exception exception)
        {
            Logger.Debug(exception);
        }

        public static void Debug(Func<FormatterDelegate, string> formatterFunc)
        {
            Logger.Debug(formatterFunc);
        }

        public static void Debug(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            Logger.Debug(formatterFunc);
        }

        public static void Debug(string text)
        {
            Logger.Debug(text);
        }

        public static void Debug(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            Logger.Debug(formatterFunc, ex);
        }

        public static void Debug(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            Logger.Debug(formatterFunc, args);
        }

        public static void Debug(Func<object[], string> textFunc, params object[] args)
        {
            Logger.Debug(textFunc, args);
        }

        public static void Debug(string text, params object[] args)
        {
            Logger.Debug(text, args);
        }

        public static void Error(Func<string> textFunc)
        {
            Logger.Error(textFunc);
        }

        public static void Error(Exception exception)
        {
            Logger.Error(exception);
        }

        public static void Error(Func<FormatterDelegate, string> formatterFunc)
        {
            Logger.Error(formatterFunc);
        }

        public static void Error(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            Logger.Error(formatterFunc);
        }

        public static void Error(string text)
        {
            Logger.Error(text);
        }

        public static void Error(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            Logger.Error(formatterFunc, ex);
        }

        public static void Error(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            Logger.Error(formatterFunc, args);
        }

        public static void Error(Func<object[], string> textFunc, params object[] args)
        {
            Logger.Error(textFunc, args);
        }

        public static void Error(string text, params object[] args)
        {
            Logger.Error(text, args);
        }

        public static ILogger ForContext(string context)
        {
            if (_logger != null)
                return new ContexualPrefixLogger(context, _loggerBase);
            throw InvalidLoggerException();
        }

        public static ILogger ForContext(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (_logger != null)
            {
                var context = type.Name;
                return new ContexualPrefixLogger(context, _loggerBase);
            }
            throw InvalidLoggerException();
        }

        public static ILogger ForContext<T>(bool useFullName = false)
        {
            if (_logger != null)
            {
                var context = useFullName ? typeof (T).FullName : typeof (T).Name;
                return new ContexualPrefixLogger(context, _loggerBase);
            }
            throw InvalidLoggerException();
        }

        public static void Info(Func<string> textFunc)
        {
            Logger.Info(textFunc);
        }

        public static void Info(Exception exception)
        {
            Logger.Info(exception);
        }

        public static void Info(Func<FormatterDelegate, string> formatterFunc)
        {
            Logger.Info(formatterFunc);
        }

        public static void Info(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            Logger.Info(formatterFunc);
        }

        public static void Info(string text)
        {
            Logger.Info(text);
        }

        public static void Info(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            Logger.Info(formatterFunc, ex);
        }

        public static void Info(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            Logger.Info(formatterFunc, args);
        }

        public static void Info(Func<object[], string> textFunc, params object[] args)
        {
            Logger.Info(textFunc, args);
        }

        public static void Info(string text, params object[] args)
        {
            Logger.Info(text, args);
        }

        public static void Trace(Func<string> textFunc)
        {
            Logger.Trace(textFunc);
        }

        public static void Trace(Exception exception)
        {
            Logger.Trace(exception);
        }

        public static void Trace(Func<FormatterDelegate, string> formatterFunc)
        {
            Logger.Trace(formatterFunc);
        }

        public static void Trace(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            Logger.Trace(formatterFunc);
        }

        public static void Trace(string text)
        {
            Logger.Trace(text);
        }

        public static void Trace(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            Logger.Trace(formatterFunc, ex);
        }

        public static void Trace(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            Logger.Trace(formatterFunc, args);
        }

        public static void Trace(Func<object[], string> textFunc, params object[] args)
        {
            Logger.Trace(textFunc, args);
        }

        public static void Trace(string text, params object[] args)
        {
            Logger.Trace(text, args);
        }

        public static void Warn(Func<string> textFunc)
        {
            Logger.Warn(textFunc);
        }

        public static void Warn(Exception exception)
        {
            Logger.Warn(exception);
        }

        public static void Warn(Func<FormatterDelegate, string> formatterFunc)
        {
            Logger.Warn(formatterFunc);
        }

        public static void Warn(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            Logger.Warn(formatterFunc);
        }

        public static void Warn(string text)
        {
            Logger.Warn(text);
        }

        public static void Warn(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            Logger.Warn(formatterFunc, ex);
        }

        public static void Warn(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            Logger.Warn(formatterFunc, args);
        }

        public static void Warn(Func<object[], string> textFunc, params object[] args)
        {
            Logger.Warn(textFunc, args);
        }

        public static void Warn(string text, params object[] args)
        {
            Logger.Warn(text, args);
        }

        private static InvalidOperationException InvalidLoggerException()
        {
            return new InvalidOperationException("Only loggers derived from LoggerBase supports contexual logging.");
        }
    }
}
