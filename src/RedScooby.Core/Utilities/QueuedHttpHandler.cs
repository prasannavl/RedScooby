// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Punchclock;

namespace RedScooby.Utilities
{
    public class QueuedHttpHandler : DelegatingHandler
    {
        private const string punchClockDefaultQueueKey = "__NONE__";
        private readonly int priority;
        private readonly OperationQueue queue;
        private int maxConcurrentOperations;

        public QueuedHttpHandler(int maxConcurrentOperations = 4,
            NetworkDataPriority priority = NetworkDataPriority.Normal)
        {
            this.maxConcurrentOperations = maxConcurrentOperations;
            queue = new OperationQueue(ConcurrencyLevel);
            this.priority = (int) priority;
        }

        public int ConcurrencyLevel
        {
            get { return maxConcurrentOperations; }
            set
            {
                maxConcurrentOperations = value;
                queue.SetMaximumConcurrent(value);
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Enqueue(() => base.SendAsync(request, cancellationToken), punchClockDefaultQueueKey,
                cancellationToken);
        }

        private Task<T> Enqueue<T>(Func<Task<T>> asyncAction, string concurrencyGroupKey,
            CancellationToken cancellationToken)
        {
            return queue.Enqueue(priority, concurrencyGroupKey, cancellationToken,
                asyncAction);
        }
    }
}
