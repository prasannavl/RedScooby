// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Logging.Core
{
    public class LoggerHook : ILoggerControl, ILoggerImpl
    {
        private readonly ILoggerImpl logger;

        protected LoggerHook(ILoggerImpl logger)
        {
            this.logger = logger;
        }

        public void Dispose()
        {
            throw FacadeInvalidOperationException();
        }

        public void Execute(LogLevel level, string message)
        {
            throw FacadeInvalidOperationException();
        }

        public void Critical(string text)
        {
            Hook(ref text);
            logger.Critical(text);
        }

        public void Critical(string text, params object[] args)
        {
            Hook(ref text, ref args);
            logger.Critical(text, args);
        }

        public void Critical(Func<string> textFunc)
        {
            Hook(ref textFunc);
            logger.Critical(textFunc);
        }

        public void Critical(Func<object[], string> textFunc, params object[] args)
        {
            Hook(ref textFunc, ref args);
            logger.Critical(textFunc, args);
        }

        public void Critical(Exception exception)
        {
            Hook(ref exception);
            logger.Critical(exception);
        }

        public void Critical(Func<FormatterDelegate, string> formatterFunc)
        {
            Hook(ref formatterFunc);
            logger.Critical(formatterFunc);
        }

        public void Critical(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            Hook(ref formatterFunc, ref args);
            logger.Critical(formatterFunc, args);
        }

        public void Critical(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            Hook(ref formatterFunc);
            logger.Critical(formatterFunc);
        }

        public void Critical(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            Hook(ref formatterFunc, ref ex);
            logger.Critical(formatterFunc, ex);
        }

        public void Error(string text)
        {
            Hook(ref text);
            logger.Error(text);
        }

        public void Error(string text, params object[] args)
        {
            Hook(ref text, ref args);
            logger.Error(text, args);
        }

        public void Error(Func<string> textFunc)
        {
            Hook(ref textFunc);
            logger.Error(textFunc);
        }

        public void Error(Func<object[], string> textFunc, params object[] args)
        {
            Hook(ref textFunc, ref args);
            logger.Error(textFunc, args);
        }

        public void Error(Func<FormatterDelegate, string> formatterFunc)
        {
            Hook(ref formatterFunc);
            logger.Error(formatterFunc);
        }

        public void Error(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            Hook(ref formatterFunc, ref args);
            logger.Error(formatterFunc, args);
        }

        public void Error(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            Hook(ref formatterFunc);
            logger.Error(formatterFunc);
        }

        public void Error(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            Hook(ref formatterFunc, ref ex);
            logger.Error(formatterFunc, ex);
        }

        public void Error(Exception exception)
        {
            Hook(ref exception);
            logger.Error(exception);
        }

        public void Warn(string text)
        {
            Hook(ref text);
            logger.Warn(text);
        }

        public void Warn(string text, params object[] args)
        {
            Hook(ref text, ref args);
            logger.Warn(text, args);
        }

        public void Warn(Func<string> textFunc)
        {
            Hook(ref textFunc);
            logger.Warn(textFunc);
        }

        public void Warn(Func<object[], string> textFunc, params object[] args)
        {
            Hook(ref textFunc, ref args);
            logger.Warn(textFunc, args);
        }

        public void Warn(Exception exception)
        {
            Hook(ref exception);
            logger.Warn(exception);
        }

        public void Warn(Func<FormatterDelegate, string> formatterFunc)
        {
            Hook(ref formatterFunc);
            logger.Warn(formatterFunc);
        }

        public void Warn(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            Hook(ref formatterFunc, ref args);
            logger.Warn(formatterFunc, args);
        }

        public void Warn(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            Hook(ref formatterFunc);
            logger.Warn(formatterFunc);
        }

        public void Warn(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            Hook(ref formatterFunc, ref ex);
            logger.Warn(formatterFunc, ex);
        }

        public void Info(string text)
        {
            Hook(ref text);
            logger.Info(text);
        }

        public void Info(string text, params object[] args)
        {
            Hook(ref text, ref args);
            logger.Info(text, args);
        }

        public void Info(Func<string> textFunc)
        {
            Hook(ref textFunc);
            logger.Info(textFunc);
        }

        public void Info(Func<object[], string> textFunc, params object[] args)
        {
            Hook(ref textFunc, ref args);
            logger.Info(textFunc, args);
        }

        public void Info(Func<FormatterDelegate, string> formatterFunc)
        {
            Hook(ref formatterFunc);
            logger.Info(formatterFunc);
        }

        public void Info(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            Hook(ref formatterFunc, ref args);
            logger.Info(formatterFunc, args);
        }

        public void Info(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            Hook(ref formatterFunc);
            logger.Info(formatterFunc);
        }

        public void Info(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            Hook(ref formatterFunc, ref ex);
            logger.Info(formatterFunc, ex);
        }

        public void Info(Exception exception)
        {
            Hook(ref exception);
            logger.Info(exception);
        }

        public void Debug(string text)
        {
            Hook(ref text);
            logger.Debug(text);
        }

        public void Debug(string text, params object[] args)
        {
            Hook(ref text, ref args);
            logger.Debug(text, args);
        }

        public void Debug(Func<string> textFunc)
        {
            Hook(ref textFunc);
            logger.Debug(textFunc);
        }

        public void Debug(Func<object[], string> textFunc, params object[] args)
        {
            Hook(ref textFunc, ref args);
            logger.Debug(textFunc, args);
        }

        public void Debug(Func<FormatterDelegate, string> formatterFunc)
        {
            Hook(ref formatterFunc);
            logger.Debug(formatterFunc);
        }

        public void Debug(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            Hook(ref formatterFunc, ref args);
            logger.Debug(formatterFunc, args);
        }

        public void Debug(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            Hook(ref formatterFunc);
            logger.Debug(formatterFunc);
        }

        public void Debug(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            Hook(ref formatterFunc, ref ex);
            logger.Debug(formatterFunc, ex);
        }

        public void Debug(Exception exception)
        {
            Hook(ref exception);
            logger.Debug(exception);
        }

        public void Trace(string text)
        {
            Hook(ref text);
            logger.Trace(text);
        }

        public void Trace(string text, params object[] args)
        {
            Hook(ref text, ref args);
            logger.Trace(text, args);
        }

        public void Trace(Func<string> textFunc)
        {
            Hook(ref textFunc);
            logger.Trace(textFunc);
        }

        public void Trace(Func<object[], string> textFunc, params object[] args)
        {
            Hook(ref textFunc, ref args);
            logger.Trace(textFunc, args);
        }

        public void Trace(Func<FormatterDelegate, string> formatterFunc)
        {
            Hook(ref formatterFunc);
            logger.Trace(formatterFunc);
        }

        public void Trace(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            Hook(ref formatterFunc, ref args);
            logger.Trace(formatterFunc, args);
        }

        public void Trace(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            Hook(ref formatterFunc);
            logger.Trace(formatterFunc);
        }

        public void Trace(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            Hook(ref formatterFunc, ref ex);
            logger.Trace(formatterFunc, ex);
        }

        public void Trace(Exception exception)
        {
            Hook(ref exception);
            logger.Trace(exception);
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
            return logger.GetLogLevel();
        }

        public void SetLogLevel(LogLevel level)
        {
            throw FacadeInvalidOperationException();
        }

        public bool IsEnabled
        {
            get { return logger.IsEnabled; }
        }

        public bool IsSynchronized
        {
            get { return logger.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return logger.SyncRoot; }
        }

        public virtual void Hook(ref string text) { }
        public virtual void Hook(ref string text, ref object[] args) { }
        public virtual void Hook(ref Func<string> textFunc) { }
        public virtual void Hook(ref Func<object[], string> textFunc, ref object[] args) { }
        public virtual void Hook(ref Exception exception) { }
        public virtual void Hook(ref Func<FormatterDelegate, string> formatterFunc) { }
        public virtual void Hook(ref Func<FormatterDelegate, object[], string> formatterFunc, ref object[] args) { }
        public virtual void Hook(ref Func<ExceptionFormatterDelegate, string> formatterFunc) { }

        public virtual void Hook(ref Func<ExceptionFormatterDelegate, Exception, string> formatterFunc,
            ref Exception ex) { }

        private InvalidOperationException FacadeInvalidOperationException()
        {
            return
                new InvalidOperationException(
                    "This operation cannot be done on a facade. Perform it on the actual logger");
        }
    }
}
