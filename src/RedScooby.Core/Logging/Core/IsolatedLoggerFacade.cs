// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Logging.Core
{
    public abstract class IsolatedLoggerFacade : ILoggerControl, ILoggerImpl
    {
        private readonly ILoggerImpl logger;

        protected IsolatedLoggerFacade(ILoggerImpl logger)
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
            logger.Critical(text);
        }

        public void Critical(string text, params object[] args)
        {
            logger.Critical(text, args);
        }

        public void Critical(Func<string> textFunc)
        {
            logger.Critical(textFunc);
        }

        public void Critical(Func<object[], string> textFunc, params object[] args)
        {
            logger.Critical(textFunc, args);
        }

        public void Critical(Exception exception)
        {
            logger.Critical(exception);
        }

        public void Critical(Func<FormatterDelegate, string> formatterFunc)
        {
            logger.Critical(formatterFunc);
        }

        public void Critical(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            logger.Critical(formatterFunc, args);
        }

        public void Critical(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            logger.Critical(formatterFunc);
        }

        public void Critical(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            logger.Critical(formatterFunc, ex);
        }

        public void Error(string text)
        {
            logger.Error(text);
        }

        public void Error(string text, params object[] args)
        {
            logger.Error(text, args);
        }

        public void Error(Func<string> textFunc)
        {
            logger.Error(textFunc);
        }

        public void Error(Func<object[], string> textFunc, params object[] args)
        {
            logger.Error(textFunc, args);
        }

        public void Error(Func<FormatterDelegate, string> formatterFunc)
        {
            logger.Error(formatterFunc);
        }

        public void Error(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            logger.Error(formatterFunc, args);
        }

        public void Error(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            logger.Error(formatterFunc);
        }

        public void Error(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            logger.Error(formatterFunc, ex);
        }

        public void Error(Exception exception)
        {
            logger.Error(exception);
        }

        public void Warn(string text)
        {
            logger.Warn(text);
        }

        public void Warn(string text, params object[] args)
        {
            logger.Warn(text, args);
        }

        public void Warn(Func<string> textFunc)
        {
            logger.Warn(textFunc);
        }

        public void Warn(Func<object[], string> textFunc, params object[] args)
        {
            logger.Warn(textFunc, args);
        }

        public void Warn(Exception exception)
        {
            logger.Warn(exception);
        }

        public void Warn(Func<FormatterDelegate, string> formatterFunc)
        {
            logger.Warn(formatterFunc);
        }

        public void Warn(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            logger.Warn(formatterFunc, args);
        }

        public void Warn(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            logger.Warn(formatterFunc);
        }

        public void Warn(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            logger.Warn(formatterFunc, ex);
        }

        public void Info(string text)
        {
            logger.Info(text);
        }

        public void Info(string text, params object[] args)
        {
            logger.Info(text, args);
        }

        public void Info(Func<string> textFunc)
        {
            logger.Info(textFunc);
        }

        public void Info(Func<object[], string> textFunc, params object[] args)
        {
            logger.Info(textFunc, args);
        }

        public void Info(Func<FormatterDelegate, string> formatterFunc)
        {
            logger.Info(formatterFunc);
        }

        public void Info(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            logger.Info(formatterFunc, args);
        }

        public void Info(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            logger.Info(formatterFunc);
        }

        public void Info(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            logger.Info(formatterFunc, ex);
        }

        public void Info(Exception exception)
        {
            logger.Info(exception);
        }

        public void Debug(string text)
        {
            logger.Debug(text);
        }

        public void Debug(string text, params object[] args)
        {
            logger.Debug(text, args);
        }

        public void Debug(Func<string> textFunc)
        {
            logger.Debug(textFunc);
        }

        public void Debug(Func<object[], string> textFunc, params object[] args)
        {
            logger.Debug(textFunc, args);
        }

        public void Debug(Func<FormatterDelegate, string> formatterFunc)
        {
            logger.Debug(formatterFunc);
        }

        public void Debug(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            logger.Debug(formatterFunc, args);
        }

        public void Debug(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            logger.Debug(formatterFunc);
        }

        public void Debug(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            logger.Debug(formatterFunc, ex);
        }

        public void Debug(Exception exception)
        {
            logger.Debug(exception);
        }

        public void Trace(string text)
        {
            logger.Trace(text);
        }

        public void Trace(string text, params object[] args)
        {
            logger.Trace(text, args);
        }

        public void Trace(Func<string> textFunc)
        {
            logger.Trace(textFunc);
        }

        public void Trace(Func<object[], string> textFunc, params object[] args)
        {
            logger.Trace(textFunc, args);
        }

        public void Trace(Func<FormatterDelegate, string> formatterFunc)
        {
            logger.Trace(formatterFunc);
        }

        public void Trace(Func<FormatterDelegate, object[], string> formatterFunc, params object[] args)
        {
            logger.Trace(formatterFunc, args);
        }

        public void Trace(Func<ExceptionFormatterDelegate, string> formatterFunc)
        {
            logger.Trace(formatterFunc);
        }

        public void Trace(Func<ExceptionFormatterDelegate, Exception, string> formatterFunc, Exception ex)
        {
            logger.Trace(formatterFunc, ex);
        }

        public void Trace(Exception exception)
        {
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
            throw FacadeInvalidOperationException();
        }

        public void SetLogLevel(LogLevel level)
        {
            throw FacadeInvalidOperationException();
        }

        public bool IsEnabled
        {
            get { throw FacadeInvalidOperationException(); }
        }

        public bool IsSynchronized
        {
            get { throw FacadeInvalidOperationException(); }
        }

        public object SyncRoot
        {
            get { throw FacadeInvalidOperationException(); }
        }

        private InvalidOperationException FacadeInvalidOperationException()
        {
            return
                new InvalidOperationException(
                    "This operation cannot be done on a facade. Perform it on the actual logger");
        }
    }
}
