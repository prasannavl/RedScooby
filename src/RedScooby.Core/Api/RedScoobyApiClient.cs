// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Api.Core;
using RedScooby.Api.Endpoints;

namespace RedScooby.Api
{
    public class RedScoobyApiClient : RedscoobyApiEndpoint
    {
        private readonly RedScoobyApiConfiguration configuration;
        private AccountEndpoint account;
        private DistressEndpoint distress;
        private ConcernEndpoint concern;
        private MeEndpoint me;

        internal RedScoobyApiClient(RedScoobyApiConfiguration config, RedScoobyHttpClient httpClient, string baseUrl)
            : base(config, httpClient, baseUrl)
        {
            configuration = config;
        }

        public AccountEndpoint Account
        {
            get { return account ?? (account = new AccountEndpoint(this)); }
        }

        public DistressEndpoint Distress
        {
            get { return distress ?? (distress = new DistressEndpoint(this)); }
        }

        public ConcernEndpoint Concern
        {
            get { return concern ?? (concern = new ConcernEndpoint(this)); }
        }

        public MeEndpoint Me
        {
            get { return me ?? (me = new MeEndpoint(this)); }
        }

        public override void Dispose()
        {
            Client.Dispose();
        }

        public RedScoobyApiConfiguration GetConfiguration()
        {
            return configuration;
        }
    }
}
