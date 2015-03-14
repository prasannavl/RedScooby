// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using RedScooby.Logging.Core;

namespace RedScooby.Logging
{
    //TODO: Reimplement logging with just an IFormatProvider overload, instead of all the delegate methods, to greatly simply logging, and yet retain its feature set.
    public interface ILogger
    {
        void Critical(string text);
        void Critical(string text, params object[] args);
        void Critical(Func<string> textFunc);
        void Critical(Func<object[], string> textFunc, params object[] args);
        void Critical(Exception exception);
        void Critical(Func<FormatterDelegate, string> formatterFunc);
        void Critical(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args);
        void Critical(Func<ExceptionFormatterDelegate, string> formatterFunc);
        void Critical(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex);
        void Debug(string text);
        void Debug(string text, params object[] args);
        void Debug(Func<string> textFunc);
        void Debug(Func<object[], string> textFunc, params object[] args);
        void Debug(Func<FormatterDelegate, string> formatterFunc);
        void Debug(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args);
        void Debug(Func<ExceptionFormatterDelegate, string> formatterFunc);
        void Debug(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex);
        void Debug(Exception exception);
        void Error(string text);
        void Error(string text, params object[] args);
        void Error(Func<string> textFunc);
        void Error(Func<object[], string> textFunc, params object[] args);
        void Error(Func<FormatterDelegate, string> formatterFunc);
        void Error(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args);
        void Error(Func<ExceptionFormatterDelegate, string> formatterFunc);
        void Error(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex);
        void Error(Exception exception);
        void Info(string text);
        void Info(string text, params object[] args);
        void Info(Func<string> textFunc);
        void Info(Func<object[], string> textFunc, params object[] args);
        void Info(Func<FormatterDelegate, string> formatterFunc);
        void Info(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args);
        void Info(Func<ExceptionFormatterDelegate, string> formatterFunc);
        void Info(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex);
        void Info(Exception exception);
        void Trace(string text);
        void Trace(string text, params object[] args);
        void Trace(Func<string> textFunc);
        void Trace(Func<object[], string> textFunc, params object[] args);
        void Trace(Func<FormatterDelegate, string> formatterFunc);
        void Trace(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args);
        void Trace(Func<ExceptionFormatterDelegate, string> formatterFunc);
        void Trace(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex);
        void Trace(Exception exception);
        void Warn(string text);
        void Warn(string text, params object[] args);
        void Warn(Func<string> textFunc);
        void Warn(Func<object[], string> textFunc, params object[] args);
        void Warn(Exception exception);
        void Warn(Func<FormatterDelegate, string> formatterFunc);
        void Warn(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args);
        void Warn(Func<ExceptionFormatterDelegate, string> formatterFunc);
        void Warn(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex);
    }

    public interface ILoggerImpl : ILogger, ILoggerControl, ILogExecutor { }

    public interface ILogExecutor
    {
        void Execute(LogLevel level, string message);
    }

    public delegate string FormatterDelegate(string message, params object[] args);

    public delegate string ExceptionFormatterDelegate(Exception ex);
}
