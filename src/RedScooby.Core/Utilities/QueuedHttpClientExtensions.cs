// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace RedScooby.Utilities
{
    public static class QueuedHttpClientExtensions
    {
        public static MediaTypeHeaderValue BuildHeaderValue(string mediaType)
        {
            return mediaType != null ? new MediaTypeHeaderValue(mediaType) : null;
        }

        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this QueuedHttpClient client, string requestUri,
            T value,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsJsonAsync(client, requestUri, value, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this QueuedHttpClient client, string requestUri,
            T value,
            CancellationToken cancellationToken, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(client, requestUri, value,
                new JsonMediaTypeFormatter(), cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsJsonAsync(client, requestUri, value, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            CancellationToken cancellationToken, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(client, requestUri, value,
                new JsonMediaTypeFormatter(), cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsXmlAsync<T>(this QueuedHttpClient client, string requestUri,
            T value,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsXmlAsync(client, requestUri, value, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsXmlAsync<T>(this QueuedHttpClient client, string requestUri,
            T value,
            CancellationToken cancellationToken, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(client, requestUri, value,
                new XmlMediaTypeFormatter(), cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsXmlAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsXmlAsync(client, requestUri, value, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsXmlAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            CancellationToken cancellationToken, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(client, requestUri, value,
                new XmlMediaTypeFormatter(), cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsync<T>(this QueuedHttpClient client, string requestUri, T value,
            MediaTypeFormatter formatter, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(client, requestUri, value, formatter, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsync<T>(this QueuedHttpClient client, string requestUri, T value,
            MediaTypeFormatter formatter, CancellationToken cancellationToken, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(client, requestUri, value, formatter, (MediaTypeHeaderValue) null,
                cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsync<T>(this QueuedHttpClient client, string requestUri, T value,
            MediaTypeFormatter formatter, string mediaType, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(client, requestUri, value, formatter, mediaType,
                CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsync<T>(this QueuedHttpClient client, string requestUri, T value,
            MediaTypeFormatter formatter, string mediaType, CancellationToken cancellationToken,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(client, requestUri, value, formatter,
                BuildHeaderValue(mediaType), cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsync<T>(this QueuedHttpClient client, string requestUri, T value,
            MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType, CancellationToken cancellationToken,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            if (client == null) throw new ArgumentNullException("client");

            var objectContent = new ObjectContent<T>(value, formatter, mediaType);
            return client.PostAsync(requestUri, objectContent, cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            MediaTypeFormatter formatter, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(client, requestUri, value, formatter, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            MediaTypeFormatter formatter, CancellationToken cancellationToken, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(client, requestUri, value, formatter, (MediaTypeHeaderValue) null,
                cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            MediaTypeFormatter formatter, string mediaType, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(client, requestUri, value, formatter, mediaType,
                CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            MediaTypeFormatter formatter, string mediaType, CancellationToken cancellationToken,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PostAsync(client, requestUri, value, formatter,
                BuildHeaderValue(mediaType), cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PostAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType, CancellationToken cancellationToken,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            if (client == null) throw new ArgumentNullException("client");

            var objectContent = new ObjectContent<T>(value, formatter, mediaType);
            return client.PostAsync(requestUri, objectContent, cancellationToken, priority, concurrencyGroupKey);
        }

        /// <typeparam name="T" />
        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this QueuedHttpClient client, string requestUri,
            T value,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsJsonAsync(client, requestUri, value, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this QueuedHttpClient client, string requestUri,
            T value,
            CancellationToken cancellationToken, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(client, requestUri, value,
                new JsonMediaTypeFormatter(), cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsJsonAsync(client, requestUri, value, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            CancellationToken cancellationToken, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(client, requestUri, value,
                new JsonMediaTypeFormatter(), cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsXmlAsync<T>(this QueuedHttpClient client, string requestUri,
            T value,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsXmlAsync(client, requestUri, value, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsXmlAsync<T>(this QueuedHttpClient client, string requestUri,
            T value,
            CancellationToken cancellationToken, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(client, requestUri, value,
                new XmlMediaTypeFormatter(), cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsXmlAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsXmlAsync(client, requestUri, value, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsXmlAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            CancellationToken cancellationToken, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(client, requestUri, value,
                new XmlMediaTypeFormatter(), cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsync<T>(this QueuedHttpClient client, string requestUri, T value,
            MediaTypeFormatter formatter, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(client, requestUri, value, formatter, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsync<T>(this QueuedHttpClient client, string requestUri, T value,
            MediaTypeFormatter formatter, CancellationToken cancellationToken, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(client, requestUri, value, formatter, (MediaTypeHeaderValue) null,
                cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsync<T>(this QueuedHttpClient client, string requestUri, T value,
            MediaTypeFormatter formatter, string mediaType, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(client, requestUri, value, formatter, mediaType,
                CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsync<T>(this QueuedHttpClient client, string requestUri, T value,
            MediaTypeFormatter formatter, string mediaType, CancellationToken cancellationToken,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(client, requestUri, value, formatter,
                BuildHeaderValue(mediaType), cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsync<T>(this QueuedHttpClient client, string requestUri, T value,
            MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType, CancellationToken cancellationToken,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            if (client == null) throw new ArgumentNullException("client");

            var objectContent = new ObjectContent<T>(value, formatter, mediaType);
            return client.PutAsync(requestUri, objectContent, cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            MediaTypeFormatter formatter, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(client, requestUri, value, formatter, CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            MediaTypeFormatter formatter, CancellationToken cancellationToken, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(client, requestUri, value, formatter, (MediaTypeHeaderValue) null,
                cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            MediaTypeFormatter formatter, string mediaType, NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(client, requestUri, value, formatter, mediaType,
                CancellationToken.None, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            MediaTypeFormatter formatter, string mediaType, CancellationToken cancellationToken,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            return PutAsync(client, requestUri, value, formatter,
                BuildHeaderValue(mediaType), cancellationToken, priority, concurrencyGroupKey);
        }

        public static Task<HttpResponseMessage> PutAsync<T>(this QueuedHttpClient client, Uri requestUri, T value,
            MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType, CancellationToken cancellationToken,
            NetworkDataPriority priority,
            string concurrencyGroupKey = null)
        {
            if (client == null) throw new ArgumentNullException("client");

            var objectContent = new ObjectContent<T>(value, formatter, mediaType);
            return client.PutAsync(requestUri, objectContent, cancellationToken, priority, concurrencyGroupKey);
        }
    }
}
