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
    public class QueuedHttpClient : HttpClient
    {
        private const string punchClockDefaultQueueKey = "__NONE__";
        private readonly OperationQueue queue;
        private int maxConcurrentOperations;

        public QueuedHttpClient(int maxConcurrentOperations = 4)
        {
            this.maxConcurrentOperations = maxConcurrentOperations;
            queue = new OperationQueue(ConcurrencyLevel);
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

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return SendAsync(request, cancellationToken, NetworkDataPriority.Normal);
        }

        public Task<HttpResponseMessage> DeleteAsync(string requestUri, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return DeleteAsync(CreateUri(requestUri), priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return DeleteAsync(requestUri, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken,
            NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return DeleteAsync(CreateUri(requestUri), cancellationToken, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken,
            NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken, priority,
                concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return GetAsync(CreateUri(requestUri), priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return GetAsync(requestUri, HttpCompletionOption.ResponseContentRead, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption,
            NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return GetAsync(CreateUri(requestUri), completionOption, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption,
            NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return GetAsync(requestUri, completionOption, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken,
            NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return GetAsync(CreateUri(requestUri), cancellationToken, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken,
            NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return GetAsync(requestUri, HttpCompletionOption.ResponseContentRead, cancellationToken, priority,
                concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption,
            CancellationToken cancellationToken, NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return GetAsync(CreateUri(requestUri), completionOption, cancellationToken, priority,
                concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption,
            CancellationToken cancellationToken, NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption,
                cancellationToken, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(CreateUri(requestUri), content, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(requestUri, content, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content,
            CancellationToken cancellationToken, NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return PostAsync(CreateUri(requestUri), content, cancellationToken, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content,
            CancellationToken cancellationToken, NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = content
            }, cancellationToken, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(CreateUri(requestUri), content, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(requestUri, content, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content,
            CancellationToken cancellationToken, NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return PutAsync(CreateUri(requestUri), content, cancellationToken, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content,
            CancellationToken cancellationToken, NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = content
            }, cancellationToken, priority, concurrencyGroupKey);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return Enqueue(() => base.SendAsync(request), concurrencyGroupKey, null, priority);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken, NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return Enqueue(() => base.SendAsync(request, cancellationToken), concurrencyGroupKey, cancellationToken,
                priority);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption,
            CancellationToken cancellationToken, NetworkDataPriority priority, string concurrencyGroupKey = null)
        {
            return Enqueue(() => base.SendAsync(request, completionOption, cancellationToken), concurrencyGroupKey,
                cancellationToken,
                priority);
        }

        private Uri CreateUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return null;
            return new Uri(uri, UriKind.RelativeOrAbsolute);
        }

        private Task<T> Enqueue<T>(Func<Task<T>> asyncAction, string concurrencyGroupKey,
            CancellationToken? cancellationToken, NetworkDataPriority priority)
        {
            if (concurrencyGroupKey == null)
            {
                concurrencyGroupKey = punchClockDefaultQueueKey;
            }

            if (cancellationToken != null)
            {
                return priority == NetworkDataPriority.SkipQueue
                    ? asyncAction()
                    : queue.Enqueue((int) priority, concurrencyGroupKey, cancellationToken.Value, asyncAction);
            }

            return priority == NetworkDataPriority.SkipQueue
                ? asyncAction()
                : queue.Enqueue((int) priority, concurrencyGroupKey, asyncAction);
        }
    }
}
