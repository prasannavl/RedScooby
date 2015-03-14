// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RedScooby.Infrastructure.Messaging
{
    public interface ICoreMessenger<T>
    {
        IObservable<T> Messages { get; }
        void Send(T message);
    }

    public class CoreMessenger<T> : ICoreMessenger<T>
    {
        private readonly Subject<T> subject;

        public CoreMessenger()
        {
            subject = new Subject<T>();
            Messages = subject.AsObservable();
        }

        protected Subject<T> Subject
        {
            get { return subject; }
        }

        public void Send(T message)
        {
            Subject.OnNext(message);
        }

        public IObservable<T> Messages { get; private set; }
    }
}
