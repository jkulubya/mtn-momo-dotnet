using System;
using System.Threading.Tasks;
using Flurl.Http;

namespace MomoApi.NET
{
    public class Disbursements : BaseApi
    {
        protected override string _tokenPath { get; } = "/disbursement/token/";
        protected override string _subscriptionKey { get; }

        internal Disbursements(HttpClientFactory clientFactory, MomoConfig config) : base(clientFactory, config)
        {
            var key = config?.SubscriptionKeys?.Disbursements;
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentException("The disbursements subscription key cannot be null");
            _subscriptionKey = key;
        }
        
        public async Task<string> Transfer()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetTransaction()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetBalance()
        {
            throw new NotImplementedException();
        }

        public async Task<string> IsPayerActive()
        {
            throw new NotImplementedException();
        }
    }
}