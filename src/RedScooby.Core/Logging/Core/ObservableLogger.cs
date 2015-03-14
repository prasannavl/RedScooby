// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RedScooby.Logging.Core
{
    public interface IObservableLogger : ILoggerImpl
    {
        IObservable<LogEvent> Events { get; }
    }

    public abstract class ObservableLoggerBase : LoggerBase, IObservableLogger
    {
        protected ObservableLoggerBase()
        {
            Initialize();
        }

        protected ObservableLoggerBase(IFormatProvider formatProvider) : base(formatProvider)
        {
            if (formatProvider == null) throw new ArgumentNullException("formatProvider");

            Initialize();
        }

        private Subject<LogEvent> EventSubject { get; set; }

        public override void Execute(LogLevel level, string message)
        {
            EventSubject.OnNext(new LogEvent {Level = level, Message = message, Timestamp = DateTimeOffset.Now});
        }

        public IObservable<LogEvent> Events { get; private set; }

        private void Initialize()
        {
            EventSubject = new Subject<LogEvent>();
            Events = EventSubject.AsObservable();
        }
    }

    public sealed class ObservableLogger : ObservableLoggerBase
    {
        public ObservableLogger() { }

        public ObservableLogger(IFormatProvider formatProvider) : base(formatProvider)
        {
            if (formatProvider == null) throw new ArgumentNullException("formatProvider");
        }
    }

    public struct LogEvent
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Message { get; set; }
        public LogLevel Level { get; set; }
    }
}
