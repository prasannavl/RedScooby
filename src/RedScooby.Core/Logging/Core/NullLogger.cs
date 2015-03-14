// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Linq;

namespace RedScooby.Logging.Core
{
    public sealed class NullLogger : ILoggerControl, ILoggerImpl, IObservableLogger
    {
        public void Dispose() { }
        public void Execute(LogLevel level, string message) { }
        public void Critical(string text) { }
        public void Critical(string text, params object[] args) { }
        public void Critical(Func<string> textFunc) { }
        public void Critical(Func<object[], string> textFunc, params object[] args) { }
        public void Critical(Exception exception) { }
        public void Critical(Func<FormatterDelegate, string> formatterFunc) { }
        public void Critical(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args) { }
        public void Critical(Func<ExceptionFormatterDelegate, string> formatterFunc) { }
        public void Critical(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex) { }
        public void Error(string text) { }
        public void Error(string text, params object[] args) { }
        public void Error(Func<string> textFunc) { }
        public void Error(Func<object[], string> textFunc, params object[] args) { }
        public void Error(Func<FormatterDelegate, string> formatterFunc) { }
        public void Error(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args) { }
        public void Error(Func<ExceptionFormatterDelegate, string> formatterFunc) { }
        public void Error(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex) { }
        public void Error(Exception exception) { }
        public void Warn(string text) { }
        public void Warn(string text, params object[] args) { }
        public void Warn(Func<string> textFunc) { }
        public void Warn(Func<object[], string> textFunc, params object[] args) { }
        public void Warn(Exception exception) { }
        public void Warn(Func<FormatterDelegate, string> formatterFunc) { }
        public void Warn(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args) { }
        public void Warn(Func<ExceptionFormatterDelegate, string> formatterFunc) { }
        public void Warn(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex) { }
        public void Info(string text) { }
        public void Info(string text, params object[] args) { }
        public void Info(Func<string> textFunc) { }
        public void Info(Func<object[], string> textFunc, params object[] args) { }
        public void Info(Func<FormatterDelegate, string> formatterFunc) { }
        public void Info(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args) { }
        public void Info(Func<ExceptionFormatterDelegate, string> formatterFunc) { }
        public void Info(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex) { }
        public void Info(Exception exception) { }
        public void Debug(string text) { }
        public void Debug(string text, params object[] args) { }
        public void Debug(Func<string> textFunc) { }
        public void Debug(Func<object[], string> textFunc, params object[] args) { }
        public void Debug(Func<FormatterDelegate, string> formatterFunc) { }
        public void Debug(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args) { }
        public void Debug(Func<ExceptionFormatterDelegate, string> formatterFunc) { }
        public void Debug(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex) { }
        public void Debug(Exception exception) { }
        public void Trace(string text) { }
        public void Trace(string text, params object[] args) { }
        public void Trace(Func<string> textFunc) { }
        public void Trace(Func<object[], string> textFunc, params object[] args) { }
        public void Trace(Func<FormatterDelegate, string> formatterFunc) { }
        public void Trace(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args) { }
        public void Trace(Func<ExceptionFormatterDelegate, string> formatterFunc) { }
        public void Trace(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex) { }
        public void Trace(Exception exception) { }
        public void Enable() { }
        public void Disable() { }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Critical;
        }

        public void SetLogLevel(LogLevel level) { }

        public bool IsEnabled
        {
            get { return false; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        public IObservable<LogEvent> Events
        {
            get { return Observable.Never<LogEvent>(); }
        }
    }
}
