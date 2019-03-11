using System;
using System.Threading.Tasks;
using Flurl.Http;

namespace MomoApi.NET
{
    public class Remittances : BaseApi
    {
        protected override string _tokenPath { get; } = "/remittance/token/";
        protected override string _subscriptionKey { get; }

        internal Remittances(HttpClientFactory clientFactory, MomoConfig config) : base(clientFactory, config)
        {
            var key = config?.SubscriptionKeys?.Remittances;
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentException("The remittances subscription key cannot be null");
            _subscriptionKey = key;
        }
        
        public async Task<string> RequestToPay()
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