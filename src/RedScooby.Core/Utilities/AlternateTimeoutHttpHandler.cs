// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RedScooby.Utilities
{
    public class AlternateTimeoutHttpHandler : DelegatingHandler
    {
        public AlternateTimeoutHttpHandler(int timeoutMilliseconds)
        {
            Timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
        }

        public AlternateTimeoutHttpHandler(TimeSpan timeout)
        {
            Timeout = timeout;
        }

        public TimeSpan Timeout { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CancellationTokenSource tokenSource;
            SetTimeout(cancellationToken, out tokenSource);
            return base.SendAsync(request, tokenSource.Token);
        }

        private void SetTimeout(CancellationToken cancellationToken, out CancellationTokenSource tokenSource)
        {
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            tokenSource.CancelAfter(Timeout);
        }
    }
}
